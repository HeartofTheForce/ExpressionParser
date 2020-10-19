using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionParser.Logic.Operators;
using static ExpressionParser.Logic.CompilerOperators;

namespace ExpressionParser.Logic
{
    public class ShuntingYard
    {
        private readonly IEnumerable<Token> _infix;
        private readonly ProcessToken _processToken;
        private readonly Stack<DepthContext> _depthStack;
        private Token _previousToken;
        private int _index;

        private ShuntingYard(IEnumerable<Token> infix, ProcessToken processToken)
        {
            _infix = infix;
            _processToken = processToken;

            _depthStack = new Stack<DepthContext>();
            _depthStack.Push(new DepthContext());

            _previousToken = null;
            _index = 0;
        }

        public delegate void ProcessToken(Token token);
        public static void Process(IEnumerable<Token> infix, ProcessToken processToken)
        {
            var shuntingYard = new ShuntingYard(infix, processToken);
            shuntingYard.Process();
        }

        private void Process()
        {
            foreach (var current in _infix)
            {
                switch (current.Type)
                {
                    case TokenType.ParenthesisOpen:
                        {
                            PushDepthStack();
                        }
                        break;
                    case TokenType.ParenthesisClose:
                        {
                            if (_depthStack.Count == 1)
                                throw new MissingTokenException(TokenType.ParenthesisOpen);

                            FlushDepthStack(_processToken);

                            if (_depthStack.Peek().TryPeek(out var stackOperatorInfo) && stackOperatorInfo is FunctionOperator)
                            {
                                if (_depthStack.TryPeek(out var depthContext) && depthContext.OperandCount != stackOperatorInfo.ParameterCount)
                                    throw new ArgumentMismatchException(stackOperatorInfo, depthContext.OperandCount);

                                FlushDepthStack(_processToken);
                            }
                        }
                        break;
                    case TokenType.Delimiter:
                        {
                            if (_depthStack.Count == 1)
                                throw new MissingTokenException(TokenType.ParenthesisOpen);

                            FlushDepthStack(_processToken);
                            PushDepthStack();
                        }
                        break;
                    case TokenType.Operator:
                        {
                            var operatorInfo = ParseOperator(_previousToken, current.Value);
                            if (_depthStack.Count == 0)
                                throw new MissingTokenException(TokenType.ParenthesisOpen);

                            while (_depthStack.Peek().TryPeek(out var stackOperatorInfo) && Evaluate(operatorInfo, stackOperatorInfo))
                            {
                                var token = PopTokenize();
                                _processToken(token);
                            }

                            if (operatorInfo is FunctionOperator)
                                _depthStack.Push(new DepthContext());

                            _depthStack.Peek().Push(operatorInfo);
                        }
                        break;
                    case TokenType.Identifier:
                    case TokenType.Constant:
                        {
                            var depthContext = _depthStack.Peek();
                            depthContext.OperandCount += 1;

                            _processToken(current);
                        }
                        break;
                }

                if (current.Type != TokenType.NonSignificant)
                    _previousToken = current;

                _index += current.Value.Length;
            }

            if (_depthStack.Count > 1)
                throw new MissingTokenException(TokenType.ParenthesisClose);

            if (_depthStack.Count == 0)
                throw new MissingTokenException(TokenType.ParenthesisOpen);

            FlushDepthStack(_processToken);
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

        private void PushDepthStack()
        {
            _depthStack.Push(new DepthContext());
        }

        private void FlushDepthStack(ProcessToken processToken)
        {
            while (_depthStack.Peek().Count > 0)
            {
                var token = PopTokenize();
                processToken(token);
            }

            var depthContext = _depthStack.Pop();
            if (depthContext.OperandCount != 1)
                throw new ExpressionReductionException(depthContext.OperandCount);

            if (_depthStack.TryPeek(out var parentDepthContext))
                parentDepthContext.OperandCount += depthContext.OperandCount;
        }

        private Token PopTokenize()
        {
            var depthContext = _depthStack.Peek();
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

        private class DepthContext : Stack<IOperatorInfo>
        {
            public int OperandCount { get; set; }
            public DepthContext()
            {
                OperandCount = 0;
            }
        }

        public class ArgumentMismatchException : Exception
        {
            public IOperatorInfo OperatorInfo { get; set; }
            public int Actual { get; set; }

            public ArgumentMismatchException(IOperatorInfo operatorInfo, int actual) : base($"Argument mismatch expected {operatorInfo.ParameterCount} got {actual}")
            {
                OperatorInfo = operatorInfo;
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
