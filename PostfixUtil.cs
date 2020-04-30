using System;
using System.Collections.Generic;
using System.Text;

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
                    int b = values.Pop();
                    int a = values.Pop();

                    int result = Calculate(split[i], a, b);
                    values.Push(result);
                }
            }

            return values.Peek();
        }

        static int Calculate(string key, int a, int b)
        {
            if (OperationMap.TryGetValue(key, out var operation))
            {
                return operation(a, b);
            }

            throw new Exception("Invalid Operator");
        }


        static Dictionary<string, Func<int, int, int>> OperationMap = new Dictionary<string, Func<int, int, int>>()
        {
            { "+", (a,b) => a + b},
            { "-", (a,b) => a - b},
            { "*", (a,b) => a * b},
            { "/", (a,b) => a / b},
            { "^", (a,b) => a ^ b},
        };
    }
}
