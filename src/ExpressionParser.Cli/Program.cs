using System;
using ExpressionParser.Compilers;
using ExpressionParser.Parsers;

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

            var infixTokens = Lexer.Process(infix);

            var function = ExpressionCompiler.Compile<Context, double>(infixTokens);
            double value = function(Context.Empty);

            Console.WriteLine(value);
        }
    }
}
