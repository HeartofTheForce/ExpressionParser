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

        public delegate bool TryParse<TReturn>(string target, out TReturn value);

        public static Func<TParameter, TReturn> Compile<TParameter, TReturn>(string postfix, TryParse<TReturn> tryParse)
        {
            var param = Expression.Parameter(typeof(TParameter), "ctx");

            Expression TargetParse(string target)
            {
                if (tryParse(target, out var value))
                    return Expression.Constant(value);

                return Expression.PropertyOrField(param, target);
            }

            var expression = EvaluatePostfix(postfix, TargetParse);
            var func = Expression.Lambda<Func<TParameter, TReturn>>(expression, true, param).Compile();
            return func;
        }

        public static Expression EvaluatePostfix(string input, Func<string, Expression> targetParse)
        {
            string[] split = input.Split(' ');

            var values = new Stack<Expression>();
            for (int i = 0; i < split.Length; i++)
            {
                var operatorInfo = BinaryOperatorMap.FirstOrDefault(x => x.Output == split[i]) ?? FunctionOperatorMap.FirstOrDefault(x => x.Output == split[i]);

                Expression value;
                if (operatorInfo != null)
                {
                    if (operatorInfo.ParameterCount > values.Count)
                        throw new Exception($"Not enough arguments for {operatorInfo.Output}");

                    for (int j = operatorInfo.ParameterCount - 1; j >= 0; j--)
                    {
                        s_buffer[j] = values.Pop();
                    }

                    value = operatorInfo.Execute(s_buffer);
                }
                else
                    value = targetParse(split[i]);

                values.Push(value);
            }

            if (values.Count != 1)
                throw new Exception($"{values.Count} Remaining values");

            return values.Peek();
        }
    }
}
