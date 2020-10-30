using System;
using System.Collections.Generic;
using ExpressionParser.Operators;

namespace ExpressionParser.Parsers
{
    public class AstParser
    {
        private readonly OperatorInfo[] _operatorMap;
        private readonly IEnumerable<Token> _tokens;
        private readonly Stack<ExpressionContext> _contextStack;

        public AstParser(OperatorInfo[] operatorMap, IEnumerable<Token> tokens)
        {
            _operatorMap = operatorMap;
            _tokens = tokens;
            _contextStack = new Stack<ExpressionContext>();
            PushStack();
        }

        public static Node Parse(OperatorInfo[] operatorMap, IEnumerable<Token> tokens)
        {
            var astParser = new AstParser(operatorMap, tokens);
            return astParser.Parse();
        }

        private Node Parse()
        {
            foreach (var current in _tokens)
            {
                if (current.Type == TokenType.Operator)
                    HandleOperator(current);
                else if (_contextStack.Peek().HaveOperand())
                    HaveOperand(current);
                else
                    WantOperand(current);
            }

            return _contextStack.Peek().Reduce();
        }

        private void HandleOperator(Token current)
        {
            OperatorInfo operatorInfo;
            if (_contextStack.Peek().HaveOperand())
            {
                var postfixInfo = _operatorMap.GetPostfix(current.Value);
                var infixInfo = _operatorMap.GetInfix(current.Value);

                if (postfixInfo != null && infixInfo != null)
                    throw new Exception("Ambigous Postfix and Infix operator");

                if (postfixInfo != null)
                    operatorInfo = postfixInfo;
                else if (infixInfo != null)
                    operatorInfo = infixInfo;
                else
                    throw new Exception($"Invalid Postfix or Infix operator '{current.Value}'");
            }
            else
            {
                operatorInfo = _operatorMap.GetPrefix(current.Value);
                if (operatorInfo == null)
                    throw new Exception($"Invalid Prefix operator '{current.Value}'");
            }

            _contextStack.Peek().PushOperator(operatorInfo);
        }

        private void WantOperand(Token current)
        {
            switch (current.Type)
            {
                case TokenType.ParenthesisOpen:
                    {
                        PushStack();
                    }
                    break;
                case TokenType.Identifier:
                case TokenType.Constant:
                    {
                        _contextStack.Peek().PushOperand(new LeafNode(current.Value));
                    }
                    break;
                default: throw new ExpressionTermException(current.Type);
            }
        }

        private void HaveOperand(Token current)
        {
            switch (current.Type)
            {
                case TokenType.ParenthesisClose:
                    {
                        if (_contextStack.Count == 1)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);

                        FlushStack();
                    }
                    break;
                case TokenType.Delimiter:
                    {
                        if (_contextStack.Count == 1)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);

                        FlushStack();
                        PushStack();
                    }
                    break;
                case TokenType.EndOfString:
                    {
                        if (_contextStack.Count > 1)
                            throw new MissingTokenException(TokenType.ParenthesisClose);

                        if (_contextStack.Count == 0)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);
                    }
                    break;
                default: throw new UnexpectedTokenException(current.Type);
            }
        }

        private void PushStack()
        {
            _contextStack.Push(new ExpressionContext());
        }

        private void FlushStack()
        {
            var expressionContext = _contextStack.Pop();
            _contextStack.Peek().PushOperand(expressionContext.Reduce());
        }
    }
}
