using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionParser.Logic.Operators;

namespace ExpressionParser.Logic
{
    public static class Compiler
    {
        public static readonly IOperatorInfo<Expression>[] BinaryOperatorMap = new BinaryOperator[]
        {
            new BinaryOperator() { Input = "*", Output = "*", Precedence = 5, BinaryExpression = Expression.Multiply},
            new BinaryOperator() { Input = "/", Output = "/", Precedence = 5, BinaryExpression = Expression.Divide},
            new BinaryOperator() { Input = "+", Output = "+", Precedence = 4, BinaryExpression = Expression.Add},
            new BinaryOperator() { Input = "-", Output = "-", Precedence = 4, BinaryExpression = Expression.Subtract},
            new BinaryOperator() { Input = "&", Output = "&", Precedence = 3, BinaryExpression = Expression.And},
            new BinaryOperator() { Input = "^", Output = "^", Precedence = 2, BinaryExpression = Expression.ExclusiveOr},
            new BinaryOperator() { Input = "|", Output = "|", Precedence = 1, BinaryExpression = Expression.Or},
        };

        public static readonly IOperatorInfo<Expression>[] FunctionOperatorMap = new IOperatorInfo<Expression>[]
        {
            new UnaryOperator() { Input = "+", Output = "u+", UnaryExpression = (arg) => arg },
            new UnaryOperator() { Input = "-", Output = "u-", UnaryExpression = Expression.Negate },
            new UnaryOperator() { Input = "~", Output = "~", UnaryExpression = Expression.Not },
            new MethodCallOperator() { Input = "max", Output = "max", ParameterCount = 2, SourceType = typeof(Math), MethodName = nameof(Math.Max) },
            new MethodCallOperator() { Input = "min", Output = "min", ParameterCount = 2, SourceType = typeof(Math), MethodName = nameof(Math.Min) },
            new MethodCallOperator() { Input = "sin", Output = "sin", ParameterCount = 1, SourceType = typeof(Math), MethodName = nameof(Math.Sin) },
            new MethodCallOperator() { Input = "cos", Output = "cos", ParameterCount = 1, SourceType = typeof(Math), MethodName = nameof(Math.Cos) },
            new MethodCallOperator() { Input = "tan", Output = "tan", ParameterCount = 1, SourceType = typeof(Math), MethodName = nameof(Math.Tan) },
        };

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
                case TokenType.Float:
                    return Expression.Constant(double.Parse(token.Value));
                case TokenType.Integer:
                    return Expression.Constant(int.Parse(token.Value));
                default:
                    throw new Exception($"Invalid TokenType: {token.Type}");
            }
        }
    }
}

