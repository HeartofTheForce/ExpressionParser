using System;
using System.Collections.Generic;

namespace ExpressionParser
{
    public static class PostfixUtil
    {
        public static int EvaluatePostfix(string input)
        {
            var split = input.Split(' ');

            Stack<int> values = new Stack<int>();
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
            if (BinaryOperationMap.TryGetValue(key, out var binaryOperation))
            {
                int b = values.Pop();
                int a = values.Pop();
                return binaryOperation(a, b);
            }
            else if (UnaryOperationMap.TryGetValue(key, out var unaryOperation))
            {
                int a = values.Pop();
                return unaryOperation(a);
            }

            throw new Exception("Invalid Operator");
        }


        static Dictionary<string, Func<int, int, int>> BinaryOperationMap = new Dictionary<string, Func<int, int, int>>()
        {
            { "+", (a,b) => a + b},
            { "-", (a,b) => a - b},
            { "*", (a,b) => a * b},
            { "/", (a,b) => a / b},
            { "^", (a,b) => a ^ b},
        };

        static Dictionary<string, Func<int, int>> UnaryOperationMap = new Dictionary<string, Func<int, int>>()
        {
            { "u+", (a) => a},
            { "u-", (a) => -a},
            { "~", (a) => ~a},
        };
    }
}
