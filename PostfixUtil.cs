using System;
using System.Collections.Generic;

namespace ExpressionParser
{
    public static class PostfixUtil
    {
        public static int EvaluatePostfix(string input)
        {
            string[] split = input.Split(' ');

            var values = new Stack<int>();
            for (int i = 0; i < split.Length; i++)
            {
                if (int.TryParse(split[i], out int value))
                {
                    values.Push(value);
                }
                else
                {
                    int result = Calculate(split[i], values);
                    values.Push(result);
                }
            }

            return values.Peek();
        }

        static int Calculate(string key, Stack<int> values)
        {
            if (s_binaryOperationMap.TryGetValue(key, out var binaryOperation))
            {
                int b = values.Pop();
                int a = values.Pop();
                return binaryOperation(a, b);
            }
            else if (s_unaryOperationMap.TryGetValue(key, out var unaryOperation))
            {
                int a = values.Pop();
                return unaryOperation(a);
            }

            throw new Exception("Invalid Operator");
        }


        static readonly Dictionary<string, Func<int, int, int>> s_binaryOperationMap = new Dictionary<string, Func<int, int, int>>()
        {
            { "+", (a,b) => a + b},
            { "-", (a,b) => a - b},
            { "*", (a,b) => a * b},
            { "/", (a,b) => a / b},
            { "^", (a,b) => a ^ b},
        };

        static readonly Dictionary<string, Func<int, int>> s_unaryOperationMap = new Dictionary<string, Func<int, int>>()
        {
            { "u+", (a) => a},
            { "u-", (a) => -a},
            { "~", (a) => ~a},
        };
    }
}
