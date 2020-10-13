using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionParser.Logic.Operators;

namespace ExpressionParser.Logic
{
    public static class Infix
    {
        public static IEnumerable<Token> Infix2Postfix(IEnumerable<Token> infix)
        {
            var postfix = new List<Token>();

            var depthStack = new Stack<Stack<IOperatorInfo>>();
            depthStack.Push(new Stack<IOperatorInfo>());

            int parameterCount = 0;

            Token previousToken = null;

            int i = 0;
            foreach (var current in infix)
            {
                switch (current.Type)
                {
                    case TokenType.NonSignificant: continue;
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
                                postfix.Add(PopDepthStack(ref parameterCount, depthStack));
                            }
                            depthStack.Pop();
                        }
                        break;
                    case TokenType.Operator:
                        {
                            var operatorInfo = ParseOperator(previousToken, current.Value);
                            if (depthStack.Count == 0)
                                throw new Exception($"( Expected: {i}");

                            while (depthStack.Peek().Count > 0 && Evaluate(operatorInfo, depthStack.Peek().Peek()))
                            {
                                postfix.Add(PopDepthStack(ref parameterCount, depthStack));
                            }

                            depthStack.Peek().Push(operatorInfo);
                        }
                        break;
                    case TokenType.Identifier:
                    case TokenType.Integer:
                    case TokenType.Float:
                        {
                            parameterCount++;
                            postfix.Add(current);
                        }
                        break;
                }

                if (current.Type != TokenType.NonSignificant)
                    previousToken = current;

                i++;
            }

            if (depthStack.Count > 1)
                throw new Exception($") Expected: {infix.Count() - 1}");

            if (depthStack.Count == 0)
                throw new Exception($"( Expected: {infix.Count() - 1}");

            while (depthStack.Peek().Count > 0)
            {
                postfix.Add(PopDepthStack(ref parameterCount, depthStack));
            }

            if (parameterCount > 1)
                throw new Exception($"Too many operands");

            return postfix;
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
            bool isFunction =
                previousToken == null ||
                previousToken.Type == TokenType.ParenthesisOpen ||
                previousToken.Type == TokenType.Operator;

            var functionMeta = Compiler.FunctionOperatorMap.FirstOrDefault(x => x.Input == currentOperator);
            var binaryMeta = Compiler.BinaryOperatorMap.FirstOrDefault(x => x.Input == currentOperator);

            if (isFunction)
            {
                if (functionMeta != null)
                    return functionMeta;
                else if (binaryMeta != null)
                    throw new Exception($"Incorrectly used Binary Operator: {currentOperator}");
            }
            else
            {
                if (binaryMeta != null)
                    return binaryMeta;
                else if (functionMeta != null)
                    throw new Exception($"Incorrectly used Function Operator: {currentOperator}");
            }

            throw new Exception($"Invalid Operator: {currentOperator}");
        }

        private static Token PopDepthStack(ref int parameterCount, Stack<Stack<IOperatorInfo>> depthStack)
        {
            var stackOperatorInfo = depthStack.Peek().Pop();
            parameterCount -= stackOperatorInfo.ParameterCount - 1;
            if (parameterCount < 1)
                throw new Exception("Too few arguments");

            return new Token(TokenType.Operator, stackOperatorInfo.Output);
        }
    }
}
