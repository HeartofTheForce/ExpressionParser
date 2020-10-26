using System.Collections.Generic;
using ExpressionParser.Parsers;

namespace ExpressionParser.Compilers
{
    public static class PostfixCompiler
    {
        public static string Compile(IEnumerable<Token> infix)
        {
            var postfix = new List<string>();
            ShuntingYard.Process(infix, (token) => postfix.Add(token.Value), (operatorInfo) => postfix.Add(operatorInfo.Output));

            return string.Join(' ', postfix);
        }
    }
}

