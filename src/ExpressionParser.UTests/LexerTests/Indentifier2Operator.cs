using System;
using System.Linq;
using ExpressionParser.Logic;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests.LexerTests
{
    [TestFixture]
    public class Indentifier2Function
    {
        static readonly LexerTestCase[] s_testCases = new LexerTestCase[]
        {
            new LexerTestCase()
            {
                Infix = "func",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Identifier, "func"),
                },
            },
            new LexerTestCase()
            {
                Infix = "func(",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "func"),
                    new Token(TokenType.ParenthesisOpen, "("),
                },
            },
            new LexerTestCase()
            {
                Infix = "func (",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "func"),
                    new Token(TokenType.ParenthesisOpen, "("),
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
