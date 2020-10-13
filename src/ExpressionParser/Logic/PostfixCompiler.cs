using System.Collections.Generic;
using System.Linq;

namespace ExpressionParser.Logic
{
    public static class PostfixCompiler
    {
        public static string Compile(IEnumerable<Token> infix)
        {
            string output = ShuntingYard<string>.Process(infix, ProcessToken);
            return output;
        }

        public static string ProcessToken(Token token, Stack<string> values)
        {
            switch (token.Type)
            {
                case TokenType.Operator:
                    {
                        var postfix = new List<string>() { token.Value };

                        while (values.Count > 0)
                        {
                            postfix.Insert(0, values.Pop());
                        }

                        return string.Join(' ', postfix);
                    }
                default:
                    return token.Value;
            }

        }
    }
}

