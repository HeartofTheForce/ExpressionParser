using ExpressionParser.Parsers;
using NUnit.Framework;

namespace ExpressionParser.UTests.ShuntingYardTests
{
    [TestFixture]
    public class MissingToken
    {
        static readonly MissingTokenTestCase[] s_testCases = new MissingTokenTestCase[]
        {
            //MissingOpen
            new MissingTokenTestCase()
            {
                Infix = "(1+2))",
                ExpectedType = TokenType.ParenthesisOpen,
            },
            //MissingClose
            new MissingTokenTestCase()
            {
                Infix = "(1+2",
                ExpectedType = TokenType.ParenthesisClose,
            },
            //MissingCloseFunction
            new MissingTokenTestCase()
            {
                Infix = "max(1,2",
                ExpectedType = TokenType.ParenthesisClose,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(MissingTokenTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var ex = Assert.Throws<ShuntingYard.MissingTokenException>(() => ShuntingYard.Process(tokens, (token) => { }));

            Assert.AreEqual(testCase.ExpectedType, ex.Type);
        }

        public struct MissingTokenTestCase
        {
            public string Infix { get; set; }
            public TokenType ExpectedType { get; set; }
        }
    }
}
