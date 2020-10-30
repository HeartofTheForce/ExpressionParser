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

            var tokens = Lexer.Process(DemoUtility.OperatorMap, infix);
            var node = AstParser.Parse(DemoUtility.OperatorMap, tokens);
            var func = ExpressionCompiler.Compile<double>(DemoUtility.CompilerFunctions, node);

            Console.WriteLine(func());
        }
    }
}
