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
                    Console.WriteLine($"Actual: {postfix}");
                }
            }
        }

        public static (string, string)[] TestCases = new (string, string)[]
        {
            ("a+b*(~c^d-e)^(f+g*h)-i", "a b c ~ d ^ e - f g h * + ^ * + i -"),
            ("1^2^3", "1 2 3 ^ ^"),
            ("1^2|3", "1 2 ^ 3 |"),
            ("1|2^3", "1 2 3 ^ |"),
        };
    }
}
