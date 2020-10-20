using System.Linq;
using ExpressionParser.Logic;
using NUnit.Framework;

namespace ExpressionParser.UTests.LexerTests
{
    [TestFixture]
    public class Indentifier2Function
    {
        static readonly LexerTestCase[] s_testCases = new LexerTestCase[]
        {
            new LexerTestCase()
            {
                Infix = "max",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "max"),
                    new Token(TokenType.EndOfString, null),
                },
            },
            new LexerTestCase()
            {
                Infix = "max(",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "max"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.EndOfString, null),
                },
            },
            new LexerTestCase()
            {
                Infix = "min (",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "min"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.EndOfString, null),
                },
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(LexerTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);

            Assert.AreEqual(testCase.ExpectedTokens.Length, tokens.Count());

            int i = 0;
            foreach (var token in tokens)
            {
                Assert.AreEqual(testCase.ExpectedTokens[i], token);
                i++;
            }
        }

        public struct LexerTestCase
        {
            public string Infix { get; set; }
            public Token[] ExpectedTokens { get; set; }
        }
    }
}
