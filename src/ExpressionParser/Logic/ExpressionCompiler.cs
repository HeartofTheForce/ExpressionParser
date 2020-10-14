using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static ExpressionParser.Logic.CompilerOperators;

namespace ExpressionParser.Logic
{
    public static class ExpressionCompiler
    {
        public static Func<TParameter, TReturn> Compile<TParameter, TReturn>(IEnumerable<Token> infix)
        {
            var param = Expression.Parameter(typeof(TParameter), "ctx");

            var expression = ShuntingYard<Expression>.Process(infix, (token, values) => ProcessToken(token, param, values));
            if (expression.Type != typeof(TReturn))
                expression = Expression.Convert(expression, typeof(TReturn));

            var func = Expression.Lambda<Func<TParameter, TReturn>>(expression, true, param).Compile();
            return func;
        }

        public static Expression ProcessToken(Token token, ParameterExpression param, Stack<Expression> values)
        {
            switch (token.Type)
            {
                case TokenType.Operator:
                    {
                        var operatorInfo =
                            BinaryOperatorMap.FirstOrDefault(x => x.Output == token.Value) ??
                            FunctionOperatorMap.FirstOrDefault(x => x.Output == token.Value);

                        if (operatorInfo != null)
                        {
                            if (operatorInfo.ParameterCount > values.Count)
                                throw new Exception($"Not enough arguments: {operatorInfo.Output}");

                            var buffer = new Expression[operatorInfo.ParameterCount];
                            for (int j = operatorInfo.ParameterCount - 1; j >= 0; j--)
                            {
                                buffer[j] = values.Pop();
                            }

                            return operatorInfo.Reduce(buffer);
                        }
                        else
                            throw new Exception($"Invalid Operator: {token.Value}");
                    }
                case TokenType.Identifier:
                    return Expression.PropertyOrField(param, token.Value);
                case TokenType.Constant:
                    {
                        if (int.TryParse(token.Value, out int intValue))
                            return Expression.Constant(intValue);
                        else if (double.TryParse(token.Value, out double doubleValue))
                            return Expression.Constant(doubleValue);
                        else
                            throw new Exception($"Cannot parse {token.Type}: {token.Value}");
                    }
                default:
                    throw new Exception($"Invalid TokenType: {token.Type}");
            }
        }
    }
}

