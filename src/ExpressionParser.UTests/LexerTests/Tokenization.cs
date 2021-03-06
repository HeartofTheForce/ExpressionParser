using System.Linq;
using ExpressionParser.Parsers;
using NUnit.Framework;

namespace ExpressionParser.UTests.LexerTests
{
    [TestFixture]
    public class Tokenization
    {
        static readonly TokenizationTestCase[] s_testCases = new TokenizationTestCase[]
        {
            //NoWhitespace
            new TokenizationTestCase()
            {
                Infix = "(-1+a)",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Operator, "-"),
                    new Token(TokenType.Constant, "1"),
                    new Token(TokenType.Operator, "+"),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.ParenthesisClose, ")"),
                    new Token(TokenType.EndOfString, null),
                },
            },
            //WhitespaceExcluded
            new TokenizationTestCase()
            {
                Infix = "( - 1 + a )",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Operator, "-"),
                    new Token(TokenType.Constant, "1"),
                    new Token(TokenType.Operator, "+"),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.ParenthesisClose, ")"),
                    new Token(TokenType.EndOfString, null),
                },
            },
            //IdentifierNumberSuffix
            new TokenizationTestCase()
            {
                Infix = "a0 * a1 & 0",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Identifier, "a0"),
                    new Token(TokenType.Operator, "*"),
                    new Token(TokenType.Identifier, "a1"),
                    new Token(TokenType.Operator, "&"),
                    new Token(TokenType.Constant, "0"),
                    new Token(TokenType.EndOfString, null),
                },
            },
            //DelimiterWhitespace
            new TokenizationTestCase()
            {
                Infix = "min(1 + a , 5.3)",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "min"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Constant, "1"),
                    new Token(TokenType.Operator, "+"),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Delimiter, ","),
                    new Token(TokenType.Constant, "5.3"),
                    new Token(TokenType.ParenthesisClose, ")"),
                    new Token(TokenType.EndOfString, null),
                },
            },
            //DelimiterNoWhitespace
            new TokenizationTestCase()
            {
                Infix = "max(a,sin(c),d)",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "max"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Delimiter, ","),
                    new Token(TokenType.Operator, "sin"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.ParenthesisClose, ")"),
                    new Token(TokenType.Delimiter, ","),
                    new Token(TokenType.Identifier, "d"),
                    new Token(TokenType.ParenthesisClose, ")"),
                    new Token(TokenType.EndOfString, null),
                },
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(TokenizationTestCase testCase)
        {
            var tokens = Lexer.Process(DemoUtility.OperatorMap, testCase.Infix);

            Assert.AreEqual(testCase.ExpectedTokens.Length, tokens.Count());

            int i = 0;
            foreach (var token in tokens)
            {
                Assert.AreEqual(testCase.ExpectedTokens[i], token);
                i++;
            }
        }

        public struct TokenizationTestCase
        {
            public string Infix { get; set; }
            public Token[] ExpectedTokens { get; set; }
        }
    }
}
