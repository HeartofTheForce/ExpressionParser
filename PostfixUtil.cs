using System;
using System.Collections.Generic;
using System.Linq;
using static ExpressionParser.ExpressionRules;

namespace ExpressionParser
{
    public static class PostfixUtil
    {
        static readonly int[] s_buffer = new int[5];

        public static int EvaluatePostfix(string input, Func<string, int> parseValue)
        {
            string[] split = input.Split(' ');

            var values = new Stack<int>();
            for (int i = 0; i < split.Length; i++)
            {
                var operatorInfo = BinaryOperatorMap.FirstOrDefault(x => x.Output == split[i]) ?? FunctionOperatorMap.FirstOrDefault(x => x.Output == split[i]);

                int value;
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
                    value = parseValue(split[i]);

                values.Push(value);
            }

            if (values.Count != 1)
                throw new Exception($"{values.Count} Remaining values");

            return values.Peek();
        }
    }
}
