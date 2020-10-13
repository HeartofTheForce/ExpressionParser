using System;
using System.Linq;
using ExpressionParser.Logic;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests
{

    [TestFixture]
    public class LexerTests
    {

        static readonly LexerTestCase[] s_testCases = new LexerTestCase[]
        {
            new LexerTestCase()
            {
                Infix = "(a+b*c)",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Operator, "+"),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Operator, "*"),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.ParenthesisClose, ")"),
                },
            },
            new LexerTestCase()
            {
                Infix = "1.75*2",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Float, "1.75"),
                    new Token(TokenType.Operator, "*"),
                    new Token(TokenType.Integer, "2"),
                },
            },
            new LexerTestCase()
            {
                Infix = "func(1 + a  5.3)",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "func"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Integer, "1"),
                    new Token(TokenType.NonSignificant, " "),
                    new Token(TokenType.Operator, "+"),
                    new Token(TokenType.NonSignificant, " "),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.NonSignificant, "  "),
                    new Token(TokenType.Float, "5.3"),
                    new Token(TokenType.ParenthesisClose, ")"),
                },
            },
            new LexerTestCase()
            {
                Infix = "func(a b(c))",
                ExpectedTokens = new Token[]
                {
                    new Token(TokenType.Operator, "func"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.NonSignificant, " "),
                    new Token(TokenType.Operator, "b"),
                    new Token(TokenType.ParenthesisOpen, "("),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.ParenthesisClose, ")"),
                    new Token(TokenType.ParenthesisClose, ")"),
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
