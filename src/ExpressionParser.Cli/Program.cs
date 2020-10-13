using System;
using ExpressionParser.Logic;

namespace ExpressionParser.Cli
{
    class Program
    {
        struct Context
        {
            public static readonly Context Empty = new Context();
        }

        static void Main(string[] args)
        {
            string infix = string.Join(' ', args);
            Console.WriteLine(infix);

            var tokens = Lexer.Process(infix);
            var postfix = Infix.Infix2Postfix(tokens);

            var function = Compiler.Compile<Context, float>(postfix);
            float value = function(Context.Empty);

            Console.WriteLine(value);
        }
    }
}
