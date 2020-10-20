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
        private readonly Stack<Stack<OperatorInfo>> _operatorStack;
        private readonly Stack<int> _argStack;
        private bool _wantExpression;

        private ShuntingYard(IEnumerable<Token> infix, ProcessToken processToken)
        {
            _infix = infix;
            _processToken = processToken;

            _operatorStack = new Stack<Stack<OperatorInfo>>();
            _argStack = new Stack<int>();
            PushStack();
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
                if (_wantExpression)
                    WantExpression(current);
                else
                    HaveExpression(current);
            }
        }

        private void WantExpression(Token current)
        {
            switch (current.Type)
            {
                case TokenType.ParenthesisOpen:
                    {
                        PushStack();
                    }
                    break;
                case TokenType.Operator:
                    {
                        var operatorInfo = OperatorMap.SingleOrDefault(x => x.Input == current.Value && IsPrefix(x));
                        if (operatorInfo == null)
                            throw new Exception($"Invalid Prefix Operator: {current.Value}");

                        ProcessOperator(operatorInfo);
                    }
                    break;
                case TokenType.Identifier:
                case TokenType.Constant:
                    {
                        _wantExpression = false;
                        _argStack.Push(_argStack.Pop() + 1);
                        _processToken(current);
                    }
                    break;
                default: throw new ExpressionTermException(current.Type);
            }
        }

        private void HaveExpression(Token current)
        {
            switch (current.Type)
            {
                case TokenType.ParenthesisClose:
                    {
                        if (_operatorStack.Count == 1)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);

                        FlushStack(_processToken);
                    }
                    break;
                case TokenType.Delimiter:
                    {
                        if (_operatorStack.Count == 1)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);

                        FlushStack(_processToken);
                        PushStack();
                    }
                    break;
                case TokenType.Operator:
                    {
                        var infixInfo = OperatorMap.SingleOrDefault(x => x.Input == current.Value && IsInfix(x));
                        var postfixInfo = OperatorMap.SingleOrDefault(x => x.Input == current.Value && IsPostfix(x));

                        if (infixInfo != null && postfixInfo != null)
                            throw new Exception("Prefix and Postfix operators must be distinct");

                        OperatorInfo operatorInfo;
                        if (infixInfo != null)
                        {
                            _wantExpression = true;
                            operatorInfo = infixInfo;
                        }
                        else if (postfixInfo != null)
                            operatorInfo = postfixInfo;
                        else
                            throw new Exception($"Invalid Prefix or Postfix Operator: {current.Value}");

                        ProcessOperator(operatorInfo);
                    }
                    break;
                case TokenType.EndOfString:
                    {
                        if (_operatorStack.Count > 1)
                            throw new MissingTokenException(TokenType.ParenthesisClose);

                        if (_operatorStack.Count == 0)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);

                        FlushStack(_processToken);
                    }
                    break;
                default: throw new UnexpectedTokenException(current.Type);
            }
        }

        private void ProcessOperator(OperatorInfo operatorInfo)
        {
            if (_operatorStack.Count == 0)
                throw new MissingTokenException(TokenType.ParenthesisOpen);

            while (_operatorStack.Peek().TryPeek(out var stackOperatorInfo) && OperatorInfo.IsLowerPrecedence(operatorInfo, stackOperatorInfo))
            {
                var token = PopTokenize();
                _processToken(token);
            }

            int availableArgs = _argStack.Pop();
            if (availableArgs != operatorInfo.PreArgCount)
                throw new PreArgumentMismatchException(operatorInfo, availableArgs);

            _argStack.Push(availableArgs - operatorInfo.PreArgCount);
            _argStack.Push(0);

            _operatorStack.Peek().Push(operatorInfo);
        }

        private void PushStack()
        {
            _operatorStack.Push(new Stack<OperatorInfo>());
            _argStack.Push(0);
            _wantExpression = true;
        }

        private void FlushStack(ProcessToken processToken)
        {
            while (_operatorStack.Peek().Count > 0)
            {
                var token = PopTokenize();
                processToken(token);
            }

            int argCount = _argStack.Pop();
            if (argCount != 1)
                throw new ExpressionReductionException(argCount);

            if (_argStack.Count > 0)
                _argStack.Push(_argStack.Pop() + argCount);

            _operatorStack.Pop();
        }

        private Token PopTokenize()
        {
            var stackOperatorInfo = _operatorStack.Peek().Pop();

            int argCount = _argStack.Pop();
            if (argCount != stackOperatorInfo.PostArgCount)
                throw new PostArgumentMismatchException(stackOperatorInfo, argCount);

            _argStack.Push(_argStack.Pop() + 1);

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

        public class PostArgumentMismatchException : Exception
        {
            public OperatorInfo OperatorInfo { get; set; }
            public int Actual { get; set; }

            public PostArgumentMismatchException(OperatorInfo operatorInfo, int actual) : base($"Post-argument mismatch expected {operatorInfo.PostArgCount} got {actual}")
            {
                OperatorInfo = operatorInfo;
                Actual = actual;
            }
        }

        public class PreArgumentMismatchException : Exception
        {
            public OperatorInfo OperatorInfo { get; set; }
            public int Actual { get; set; }

            public PreArgumentMismatchException(OperatorInfo operatorInfo, int actual) : base($"Pre-argument mismatch expected {operatorInfo.PreArgCount} got {actual}")
            {
                OperatorInfo = operatorInfo;
                Actual = actual;
            }
        }

        public class ExpressionTermException : Exception
        {
            public TokenType Type { get; set; }

            public ExpressionTermException(TokenType type) : base($"Invalid Expression Term {type}")
            {
                Type = type;
            }
        }

        public class UnexpectedTokenException : Exception
        {
            public TokenType Type { get; set; }

            public UnexpectedTokenException(TokenType type) : base($"Unexpected {type}")
            {
                Type = type;
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
