using System;
using ExpressionParser.Compilers;
using ExpressionParser.Parsers;

namespace ExpressionParser.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            string infix = string.Join(' ', args);
            Console.WriteLine(infix);

            var tokens = Lexer.Process(infix);
            var node = AstParser.Parse(tokens);
            var func = ExpressionCompiler.Compile<double>(node);

            Console.WriteLine(func());
        }
    }
}
