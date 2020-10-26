using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionParser.Operators;
using static ExpressionParser.Compilers.CompilerOperators;

namespace ExpressionParser.Parsers
{
    public class ShuntingYard
    {
        private readonly IEnumerable<Token> _infix;
        private readonly ProcessOperand _processOperand;
        private readonly ProcessOperator _processOperator;

        private readonly Stack<Stack<OperatorInfo>> _operatorStack;
        private readonly Stack<int> _argStack;
        private bool _wantExpression;

        private ShuntingYard(IEnumerable<Token> infix, ProcessOperand processOperand, ProcessOperator processOperator)
        {
            _infix = infix;
            _processOperand = processOperand;
            _processOperator = processOperator;

            _operatorStack = new Stack<Stack<OperatorInfo>>();
            _argStack = new Stack<int>();
            PushStack();
        }

        public delegate void ProcessOperand(string value);
        public delegate void ProcessOperator(OperatorInfo operatorInfo);

        public static void Process(IEnumerable<Token> infix, ProcessOperand processOperand, ProcessOperator processOperator)
        {
            var shuntingYard = new ShuntingYard(infix, processOperand, processOperator);
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
                        var operatorInfo = OperatorMap.SingleOrDefault(x => x.Input == current.Value && x.IsPrefix());
                        if (operatorInfo == null)
                            throw new Exception($"Invalid Prefix Operator: {current.Value}");

                        PushOperator(operatorInfo);
                    }
                    break;
                case TokenType.Identifier:
                case TokenType.Constant:
                    {
                        _wantExpression = false;
                        _argStack.Push(_argStack.Pop() + 1);
                        _processOperand(current.Value);
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

                        FlushStack();
                    }
                    break;
                case TokenType.Delimiter:
                    {
                        if (_operatorStack.Count == 1)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);

                        FlushStack();
                        PushStack();
                    }
                    break;
                case TokenType.Operator:
                    {
                        var infixInfo = OperatorMap.SingleOrDefault(x => x.Input == current.Value && x.IsInfix());
                        var postfixInfo = OperatorMap.SingleOrDefault(x => x.Input == current.Value && x.IsPostfix());

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

                        PushOperator(operatorInfo);
                    }
                    break;
                case TokenType.EndOfString:
                    {
                        if (_operatorStack.Count > 1)
                            throw new MissingTokenException(TokenType.ParenthesisClose);

                        if (_operatorStack.Count == 0)
                            throw new MissingTokenException(TokenType.ParenthesisOpen);

                        FlushStack();
                    }
                    break;
                default: throw new UnexpectedTokenException(current.Type);
            }
        }

        private void PushOperator(OperatorInfo operatorInfo)
        {
            if (_operatorStack.Count == 0)
                throw new MissingTokenException(TokenType.ParenthesisOpen);

            while (_operatorStack.Peek().TryPeek(out var stackOperatorInfo) && OperatorInfo.IsLowerPrecedence(operatorInfo, stackOperatorInfo))
            {
                PopOperator();
            }

            int preArgs = _argStack.Pop();
            if (preArgs != operatorInfo.PreArgCount)
                throw new PreArgumentMismatchException(operatorInfo, preArgs);
            _argStack.Push(0);

            _operatorStack.Peek().Push(operatorInfo);
        }

        private void PopOperator()
        {
            var stackOperatorInfo = _operatorStack.Peek().Pop();

            int postArgs = _argStack.Pop();
            if (postArgs != stackOperatorInfo.PostArgCount)
                throw new PostArgumentMismatchException(stackOperatorInfo, postArgs);
            _argStack.Push(1);

            _processOperator(stackOperatorInfo);
        }

        private void PushStack()
        {
            _operatorStack.Push(new Stack<OperatorInfo>());
            _argStack.Push(0);
            _wantExpression = true;
        }

        private void FlushStack()
        {
            while (_operatorStack.Peek().Count > 0)
            {
                PopOperator();
            }

            int argCount = _argStack.Pop();
            if (argCount != 1)
                throw new ExpressionReductionException(argCount);

            if (_argStack.Count > 0)
                _argStack.Push(_argStack.Pop() + argCount);

            _operatorStack.Pop();
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

            public ExpressionTermException(TokenType type) : base($"Invalid Expression Term: {type}")
            {
                Type = type;
            }
        }

        public class UnexpectedTokenException : Exception
        {
            public TokenType Type { get; set; }

            public UnexpectedTokenException(TokenType type) : base($"Unexpected: {type}")
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
