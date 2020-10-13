using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static ExpressionParser.Logic.Rules;

namespace ExpressionParser.Logic
{
    public static class Compiler
    {
        static readonly Expression[] s_buffer = new Expression[2];

        public static Func<TParameter, TReturn> Compile<TParameter, TReturn>(IEnumerable<Token> postfix)
        {
            var param = Expression.Parameter(typeof(TParameter), "ctx");

            var values = new Stack<Expression>();
            foreach (var token in postfix)
            {
                values.Push(ProcessToken(token, param, values));
            }
            if (values.Count != 1)
                throw new Exception($"Remaining values: {values.Count}");

            var expression = values.Peek();
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

                            for (int j = operatorInfo.ParameterCount - 1; j >= 0; j--)
                            {
                                s_buffer[j] = values.Pop();
                            }

                            return operatorInfo.Execute(s_buffer);
                        }
                        else
                            throw new Exception($"Invalid Operator: {token.Value}");
                    }
                case TokenType.Identifier:
                    return Expression.PropertyOrField(param, token.Value);
                case TokenType.Float:
                    return Expression.Constant(float.Parse(token.Value));
                case TokenType.Integer:
                    return Expression.Constant(int.Parse(token.Value));
                default:
                    throw new Exception($"Invalid TokenType: {token.Type}");
            }
        }
    }
}

