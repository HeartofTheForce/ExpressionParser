using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionParser.Logic.Operators;
using static ExpressionParser.Logic.CompilerOperators;

namespace ExpressionParser.Logic
{
    public static class ShuntingYard<T>
    {
        public delegate void ProcessToken(Token token);

        public static void Process(IEnumerable<Token> infix, ProcessToken processToken)
        {
            var depthStack = new Stack<Stack<IOperatorInfo>>();
            depthStack.Push(new Stack<IOperatorInfo>());

            Token previousToken = null;

            int i = 0;
            foreach (var current in infix)
            {
                switch (current.Type)
                {
                    case TokenType.ParenthesisOpen:
                        {
                            depthStack.Push(new Stack<IOperatorInfo>());
                        }
                        break;
                    case TokenType.ParenthesisClose:
                        {
                            if (depthStack.Count == 1)
                                throw new Exception($"( Expected: {i}");

                            while (depthStack.Peek().Count > 0)
                            {
                                var token = PopTokenize(depthStack);
                                processToken(token);
                            }
                            depthStack.Pop();
                        }
                        break;
                    case TokenType.Delimiter:
                        {
                            if (depthStack.Count == 1)
                                throw new Exception($"( Expected: {i}");

                            while (depthStack.Peek().Count > 0)
                            {
                                var token = PopTokenize(depthStack);
                                processToken(token);
                            }
                            depthStack.Pop();
                            depthStack.Push(new Stack<IOperatorInfo>());
                        }
                        break;
                    case TokenType.Operator:
                        {
                            var operatorInfo = ParseOperator(previousToken, current.Value);
                            if (depthStack.Count == 0)
                                throw new Exception($"( Expected: {i}");

                            while (depthStack.Peek().Count > 0 && Evaluate(operatorInfo, depthStack.Peek().Peek()))
                            {
                                var token = PopTokenize(depthStack);
                                processToken(token);
                            }

                            depthStack.Peek().Push(operatorInfo);
                        }
                        break;
                    case TokenType.Identifier:
                    case TokenType.Constant:
                        {
                            processToken(current);
                        }
                        break;
                }

                if (current.Type != TokenType.NonSignificant)
                    previousToken = current;

                i += current.Value.Length;
            }

            if (depthStack.Count > 1)
                throw new Exception($") Expected: {i - 1}");

            if (depthStack.Count == 0)
                throw new Exception($"( Expected: {i - 1}");

            while (depthStack.Peek().Count > 0)
            {
                var token = PopTokenize(depthStack);
                processToken(token);
            }
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

        private static Token PopTokenize(Stack<Stack<IOperatorInfo>> depthStack)
        {
            var stackOperatorInfo = depthStack.Peek().Pop();
            return new Token(TokenType.Operator, stackOperatorInfo.Output);
        }
    }
}
