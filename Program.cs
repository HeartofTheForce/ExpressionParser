using System;
using System.Linq.Expressions;
using ExpressionParser.Parser;

namespace ExpressionParser
{
    class Program
    {
        struct Context
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
            public int D { get; set; }
            public int E { get; set; }
            public int F { get; set; }
            public int G { get; set; }
            public int H { get; set; }
            public int I { get; set; }
        }

        static void Main(string[] args)
        {
            foreach (var testCase in TestCases)
            {
                string infix = testCase.Item1;
                string postfix = Infix.Infix2Postfix(infix);
                if (postfix != testCase.Item2)
                {
                    Console.WriteLine($"Failed: {infix}");
                    Console.WriteLine($"Expected: {testCase.Item2}");
                    Console.WriteLine($"Actual  : {postfix}");
                }
                Compiler.Compile<Context, int>(postfix, TryParse);
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
            //Complex 1
            ("a+b*(~c^d-e)^(f+g*h)-i", "a b c ~ d ^ e - f g h * + ^ * + i -"),
            //Complex 2
            ("(-a ^ b | c) / (d | ~e ^ +f)","a u- b ^ c | d e ~ f u+ ^ | /"),
        };

        static bool TryParse(string target, out int value)
        {
            return int.TryParse(target, out value);
        }
    }
}
