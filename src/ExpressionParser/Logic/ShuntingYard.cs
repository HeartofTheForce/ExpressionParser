using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionParser.Logic.Operators;
using static ExpressionParser.Logic.CompilerOperators;

namespace ExpressionParser.Logic
{
    public static class ShuntingYard
    {
        private class DepthContext : Stack<IOperatorInfo>
        {
            public int OperandCount { get; set; }
            public DepthContext()
            {
                OperandCount = 0;
            }
        }

        public delegate void ProcessToken(Token token);

        public static void Process(IEnumerable<Token> infix, ProcessToken processToken)
        {
            var depthStack = new Stack<DepthContext>();
            depthStack.Push(new DepthContext());

            Token previousToken = null;

            int i = 0;
            foreach (var current in infix)
            {
                switch (current.Type)
                {
                    case TokenType.ParenthesisOpen:
                        {
                            PushDepthStack(depthStack);
                        }
                        break;
                    case TokenType.ParenthesisClose:
                        {
                            if (depthStack.Count == 1)
                                throw new MissingTokenException(TokenType.ParenthesisOpen);

                            FlushDepthStack(depthStack, processToken);

                            if (depthStack.Peek().TryPeek(out var stackOperatorInfo) && stackOperatorInfo is FunctionOperator)
                            {
                                if (depthStack.TryPeek(out var depthContext) && depthContext.OperandCount != stackOperatorInfo.ParameterCount)
                                    throw new ArgumentMismatchException(stackOperatorInfo.ParameterCount, depthContext.OperandCount);

                                FlushDepthStack(depthStack, processToken);
                            }
                        }
                        break;
                    case TokenType.Delimiter:
                        {
                            if (depthStack.Count == 1)
                                throw new MissingTokenException(TokenType.ParenthesisOpen);

                            FlushDepthStack(depthStack, processToken);
                            PushDepthStack(depthStack);
                        }
                        break;
                    case TokenType.Operator:
                        {
                            var operatorInfo = ParseOperator(previousToken, current.Value);
                            if (depthStack.Count == 0)
                                throw new MissingTokenException(TokenType.ParenthesisOpen);

                            while (depthStack.Peek().TryPeek(out var stackOperatorInfo) && Evaluate(operatorInfo, stackOperatorInfo))
                            {
                                var token = PopTokenize(depthStack);
                                processToken(token);
                            }

                            if (operatorInfo is FunctionOperator)
                                depthStack.Push(new DepthContext());

                            depthStack.Peek().Push(operatorInfo);
                        }
                        break;
                    case TokenType.Identifier:
                    case TokenType.Constant:
                        {
                            var depthContext = depthStack.Peek();
                            depthContext.OperandCount += 1;

                            processToken(current);
                        }
                        break;
                }

                if (current.Type != TokenType.NonSignificant)
                    previousToken = current;

                i += current.Value.Length;
            }

            if (depthStack.Count > 1)
                throw new MissingTokenException(TokenType.ParenthesisClose);

            if (depthStack.Count == 0)
                throw new MissingTokenException(TokenType.ParenthesisOpen);

            FlushDepthStack(depthStack, processToken);
        }

        private static bool Evaluate(IOperatorInfo current, IOperatorInfo previous)
        {
            if (current.Precedence == previous.Precedence)
            {
                if (current.Associativity == previous.Associativity)
                {
                    switch (current.Associativity)
                    {
                        case Associativity.Left: return true;
                        case Associativity.Right: return false;
                    }
                }
                else
                {
                    throw new Exception("Operators with same Precedence must have same Associativity");
                }
            }

            return current.Precedence < previous.Precedence;
        }

        private static IOperatorInfo ParseOperator(Token previousToken, string currentOperator)
        {
            bool isPrefix =
                previousToken == null ||
                previousToken.Type == TokenType.ParenthesisOpen ||
                previousToken.Type == TokenType.Delimiter ||
                previousToken.Type == TokenType.Operator;

            var prefixInfo = PrefixOperatorMap.FirstOrDefault(x => x.Input == currentOperator);
            var infixInfo = InfixOperatorMap.FirstOrDefault(x => x.Input == currentOperator);

            if (isPrefix)
            {
                if (prefixInfo != null)
                    return prefixInfo;
                else if (infixInfo != null)
                    throw new Exception($"Incorrectly used Infix Operator: {currentOperator}");
            }
            else
            {
                if (infixInfo != null)
                    return infixInfo;
                else if (prefixInfo != null)
                    throw new Exception($"Incorrectly used Prefix Operator: {currentOperator}");
            }

            throw new Exception($"Invalid Operator: {currentOperator}");
        }

        private static void PushDepthStack(Stack<DepthContext> depthStack)
        {
            depthStack.Push(new DepthContext());
        }

        private static void FlushDepthStack(Stack<DepthContext> depthStack, ProcessToken processToken)
        {
            while (depthStack.Peek().Count > 0)
            {
                var token = PopTokenize(depthStack);
                processToken(token);
            }

            var depthContext = depthStack.Pop();
            if (depthContext.OperandCount != 1)
                throw new ExpressionReductionException(depthContext.OperandCount);

            if (depthStack.TryPeek(out var parentDepthContext))
                parentDepthContext.OperandCount += depthContext.OperandCount;
        }

        private static Token PopTokenize(Stack<DepthContext> depthStack)
        {
            var depthContext = depthStack.Peek();
            var stackOperatorInfo = depthContext.Pop();

            depthContext.OperandCount -= stackOperatorInfo.ParameterCount - 1;

            return new Token(TokenType.Operator, stackOperatorInfo.Output);
        }

        public class MissingTokenException : Exception
        {
            public TokenType Type { get; }

            public MissingTokenException(TokenType type) : base($"Missing: {type}")
            {
                Type = type;
            }
        }

        public class ArgumentMismatchException : Exception
        {
            public int Expected { get; set; }
            public int Actual { get; set; }

            public ArgumentMismatchException(int expected, int actual) : base($"Argument mismatch expected {expected} got {actual}")
            {
                Expected = expected;
                Actual = actual;
            }
        }

        public class ExpressionReductionException : Exception
        {
            public int RemainingValues { get; }

            public ExpressionReductionException(int remainingValues) : base($"Expression incorrectly reduced to {remainingValues} values")
            {
                RemainingValues = remainingValues;
            }
        }
    }
}
