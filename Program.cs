using System;
namespace ExpressionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var testCase in TestCases)
            {
                string infix = testCase.Item1;
                string postfix = InfixUtil.Infix2Postfix(infix);
                if (postfix != testCase.Item2)
                {
                    Console.WriteLine($"Failed: {infix}");
                    Console.WriteLine($"Expected: {testCase.Item2}");
                    Console.WriteLine($"Actual  : {postfix}");
                }
                PostfixUtil.EvaluatePostfix(postfix, TryParse);
            }
        }

        public static (string, string)[] TestCases = new (string, string)[]
        {
            //Parentheses
            ("(a + b) * c", "a b + c *"),
            //Left Precedence
            ("a + b * c", "a b c * +"),
            //Left Associative
            ("a + b - c", "a b + c -"),
            //Right Associative
            ("a ^ b & c", "a b c & ^"),
            //Right Precedence
            ("a | b ^ c", "a b c ^ |"),
            //Associative Precedence
            ("a + b - c ^ d & e", "a b + c d e & ^ -"),
            //Unary
            ("-a + ~b", "a u- b ~ +"),
            //Unary Outside Parentheses
            ("a + ~(b + c)", "a b c + ~ +"),
             //Unary Inside Parentheses
            ("(-b)", "b u-"),
            //Unary Chained
            ("a + - ~ b", "a b ~ u- +"),
            //Function 2 parameter
            ("max a b + c", "a b max c +"),
            //Complex 1
            ("a+b*(~c^d-e)^(f+g*h)-i", "a b c ~ d ^ e - f g h * + ^ * + i -"),
            //Complex 2
            ("(-a ^ b | c) / (d | ~e ^ +f)","a u- b ^ c | d e ~ f u+ ^ | /"),
        };

        static int TryParse(string value)
        {
            if (!int.TryParse(value, out int output))
                return 1;

            return output;
        }
    }
}
