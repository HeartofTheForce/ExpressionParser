using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            // string infix = "a+b*(c^d-e)^(f+g*h)-i";
            // string postfix = InfixUtil.Infix2Postfix(infix);
            // string prefix = InfixUtil.Infix2Prefix(infix);
            // Console.WriteLine($"Postfix: {postfix}");
            // Console.WriteLine($"Prefix : {prefix}");
            // Expected Postfix abcd^e-fgh*+^*+i-

            string infix = "(9 / 3) * 4 - 5";
            string postfix = InfixUtil.Infix2Postfix(infix);

            Console.WriteLine(PostfixUtil.EvaluatePostfix(postfix));
        }
    }
}
