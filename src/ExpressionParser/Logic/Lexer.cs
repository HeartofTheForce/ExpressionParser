using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExpressionParser.Logic
{
    public static class Lexer
    {
        static readonly Pattern[] s_patterns = new Pattern[]
        {
            new Pattern()
            {
                Regex = new Regex("\\s+"),
                Type = TokenType.NonSignificant,
            },
            new Pattern()
            {
                Regex = new Regex("\\("),
                Type = TokenType.ParenthesisOpen,
            },
               new Pattern()
            {
                Regex = new Regex("\\)"),
                Type = TokenType.ParenthesisClose,
            },
            new Pattern()
            {
                Regex = new Regex("\\w+(?=\\()"),
                Type = TokenType.Operator,
            },
            new Pattern()
            {
                Regex = new Regex("[a-zA-Z]\\w*"),
                Type = TokenType.Identifier,
            },
            new Pattern()
            {
                Regex = new Regex("\\d+(?:\\.\\d+)?"),
                Type = TokenType.Constant,
            },
            new Pattern()
            {
                Regex = new Regex("[^\\s\\(\\)\\w]+"),
                Type = TokenType.Operator,
            },
        };

        private static bool TryMatch(string input, ref int offset, out Token token)
        {
            foreach (var pattern in s_patterns)
            {
                var match = pattern.Regex.Match(input, offset);
                if (match.Success && match.Index == offset)
                {
                    offset += match.Length;
                    token = new Token(pattern.Type, match.Value);
                    return true;
                }
            }

            token = null;
            return false;
        }

        public static IEnumerable<Token> Process(string input)
        {
            var output = new List<Token>();

            int offset = 0;

            while (offset < input.Length)
            {
                if (!TryMatch(input, ref offset, out var token))
                    throw new Exception($"No matching Rules: {offset}");

                output.Add(token);
            }

            return output;
        }

        private struct Pattern
        {
            public Regex Regex { get; set; }
            public TokenType Type { get; set; }
        }
    }
}
