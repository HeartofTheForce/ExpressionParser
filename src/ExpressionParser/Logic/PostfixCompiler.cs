using System.Collections.Generic;
using System.Linq;

namespace ExpressionParser.Logic
{
    public static class PostfixCompiler
    {
        public static string Compile(IEnumerable<Token> infix)
        {
            var postfix = new List<string>();
            ShuntingYard<string>.Process(infix, (token) => postfix.Add(token.Value));

            return string.Join(' ', postfix);
        }
    }
}
