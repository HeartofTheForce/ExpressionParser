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
        private bool _expectPrefix;

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
                switch (current.Type)
                {
                    case TokenType.ParenthesisOpen:
                        {
                            PushStack();
                        }
                        break;
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
                            var operatorInfo = ParseOperator(current.Value);
                            if (_operatorStack.Count == 0)
                                throw new MissingTokenException(TokenType.ParenthesisOpen);

                            while (_operatorStack.Peek().TryPeek(out var stackOperatorInfo) && Evaluate(operatorInfo, stackOperatorInfo))
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
                        break;
                    case TokenType.Identifier:
                    case TokenType.Constant:
                        {
                            _expectPrefix = false;
                            _argStack.Push(_argStack.Pop() + 1);
                            _processToken(current);
                        }
                        break;
                }
            }

            if (_operatorStack.Count > 1)
                throw new MissingTokenException(TokenType.ParenthesisClose);

            if (_operatorStack.Count == 0)
                throw new MissingTokenException(TokenType.ParenthesisOpen);

            FlushStack(_processToken);
        }

        private static bool Evaluate(OperatorInfo current, OperatorInfo previous)
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

        private OperatorInfo ParseOperator(string currentOperator)
        {
            OperatorInfo output = null;
            if (_expectPrefix)
                output = OperatorMap.SingleOrDefault(x => x.Input == currentOperator && IsPrefix(x));
            else
            {
                var infixInfo = OperatorMap.SingleOrDefault(x => x.Input == currentOperator && IsInfix(x));
                var postfixInfo = OperatorMap.SingleOrDefault(x => x.Input == currentOperator && IsPostfix(x));

                if (infixInfo != null && postfixInfo != null)
                    throw new Exception("Prefix and Postfix operators must be distinct");

                if (infixInfo != null)
                {
                    _expectPrefix = true;
                    output = infixInfo;
                }
                else
                    output = postfixInfo;
            }

            if (output == null)
                throw new Exception($"Invalid Operator: {currentOperator}");

            return output;
        }

        private void PushStack()
        {
            _operatorStack.Push(new Stack<OperatorInfo>());
            _argStack.Push(0);
            _expectPrefix = true;
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
