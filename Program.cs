using System;
namespace ExpressionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string infix = "-4 -(+(--(5)))";
            // string infix = "a+b*(~c^d-e)^-(f+g*h)-i";
            string postfix = InfixUtil.Infix2Postfix(infix);
            // string prefix = InfixUtil.Infix2Prefix(infix);
            // Console.WriteLine($"Postfix: {postfix}");
            // Console.WriteLine($"Prefix : {prefix}");
            // Expected Postfix abcd^e-fgh*+^*+i-

            Console.WriteLine(postfix);
            Console.WriteLine(PostfixUtil.EvaluatePostfix(postfix));
        }
    }
}
