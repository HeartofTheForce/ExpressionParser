using ExpressionParser.Parsers;
using NUnit.Framework;

namespace ExpressionParser.UTests.AstParserTests
{
    [TestFixture]
    public class UnexpectedToken
    {
        static readonly UnexpectedTokenTestCase[] s_testCases = new UnexpectedTokenTestCase[]
        {
            //ConstantConstant
            new UnexpectedTokenTestCase()
            {
                Infix = "1 1",
                ExpectedType = TokenType.Constant,
            },
            //IdentifierConstant
            new UnexpectedTokenTestCase()
            {
                Infix = "a 1",
                ExpectedType = TokenType.Constant,
            },
            //ConstantIdentifier
            new UnexpectedTokenTestCase()
            {
                Infix = "1 a",
                ExpectedType = TokenType.Identifier,
            },
            //IdentifierIdentifier
            new UnexpectedTokenTestCase()
            {
                Infix = "a a",
                ExpectedType = TokenType.Identifier,
            },
            //IdentifierParenthesisOpen
            new UnexpectedTokenTestCase()
            {
                Infix = "a (1)",
                ExpectedType = TokenType.ParenthesisOpen,
            },
            //ConstantParenthesisOpen
            new UnexpectedTokenTestCase()
            {
                Infix = "1 (a)",
                ExpectedType = TokenType.ParenthesisOpen,
            },
            //ParenthesisCloseIdentifier
            new UnexpectedTokenTestCase()
            {
                Infix = "(1) a",
                ExpectedType = TokenType.Identifier,
            },
            //ParenthesisCloseConstant
            new UnexpectedTokenTestCase()
            {
                Infix = "(a) 1",
                ExpectedType = TokenType.Constant,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(UnexpectedTokenTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var ex = Assert.Throws<UnexpectedTokenException>(() => AstParser.Parse(tokens));

            Assert.AreEqual(testCase.ExpectedType, ex.Type);
        }

        public struct UnexpectedTokenTestCase
        {
            public string Infix { get; set; }
            public TokenType ExpectedType { get; set; }
        }
    }
}
