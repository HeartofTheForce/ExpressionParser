using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionParser.Operators;
using ExpressionParser.Parsers;

namespace ExpressionParser.Compilers
{
    public static class ExpressionCompiler
    {
        public static Func<TParameter, TReturn> Compile<TParameter, TReturn>(IEnumerable<Token> infix)
        {
            var param = Expression.Parameter(typeof(TParameter), "ctx");
            var values = new Stack<Expression>();
            ShuntingYard.Process(infix, (token) => ProcessToken(token, param, values), (operatorInfo) => ProcessOperator(operatorInfo, values));

            if (values.Count != 1)
                throw new Exception($"Expression not fully reduced, remaining values: {values.Count}");

            var expression = values.Peek();
            if (expression.Type != typeof(TReturn))
                expression = Expression.Convert(expression, typeof(TReturn));

            var func = Expression.Lambda<Func<TParameter, TReturn>>(expression, true, param).Compile();
            return func;
        }

        public static void ProcessOperator(OperatorInfo operatorInfo, Stack<Expression> values)
        {
            if (operatorInfo.ArgCount > values.Count)
                throw new Exception($"Not enough arguments: {operatorInfo.Output}");

            var buffer = new Expression[operatorInfo.ArgCount];
            for (int j = operatorInfo.ArgCount - 1; j >= 0; j--)
            {
                buffer[j] = values.Pop();
            }

            if (operatorInfo is IReducible<Expression> reducible)
                values.Push(reducible.Reduce(buffer));
            else
                throw new Exception($"Cannot reduce: {operatorInfo.Output}");
        }

        public static void ProcessToken(Token token, ParameterExpression param, Stack<Expression> values)
        {
            Expression expression;
            switch (token.Type)
            {
                case TokenType.Identifier:
                    expression = Expression.PropertyOrField(param, token.Value);
                    break;
                case TokenType.Constant:
                    {
                        if (int.TryParse(token.Value, out int intValue))
                            expression = Expression.Constant(intValue);
                        else if (double.TryParse(token.Value, out double doubleValue))
                            expression = Expression.Constant(doubleValue);
                        else
                            throw new Exception($"Cannot parse {token.Type}: {token.Value}");
                    }
                    break;
                default:
                    throw new Exception($"Invalid TokenType: {token.Type}");
            }

            values.Push(expression);
        }
    }
}

