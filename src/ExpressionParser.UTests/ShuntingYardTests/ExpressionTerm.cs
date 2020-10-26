using ExpressionParser.Parsers;
using NUnit.Framework;

namespace ExpressionParser.UTests.ShuntingYardTests
{
    [TestFixture]
    public class ExpressionTerm
    {
        static readonly ExpressionTermTestCase[] s_testCases = new ExpressionTermTestCase[]
        {
            //EmptyDelimiterClose
            new ExpressionTermTestCase()
            {
                Infix = "max(1,2,)",
                ExpectedType = TokenType.ParenthesisClose,
            },
            //EmptyDelimiterDelimiter
            new ExpressionTermTestCase()
            {
                Infix = "max(1,,3)",
                ExpectedType = TokenType.Delimiter,
            },
            //EmptyOpenDelimiter
            new ExpressionTermTestCase()
            {
                Infix = "max(,2,3)",
                ExpectedType = TokenType.Delimiter,
            },
            //EmptyParentheses
            new ExpressionTermTestCase()
            {
                Infix = "()",
                ExpectedType = TokenType.ParenthesisClose,
            },
            //UnaryNoOperand
            new ExpressionTermTestCase()
            {
                Infix = "max(1, -)",
                ExpectedType = TokenType.ParenthesisClose,
            },
            
            //TooFewArgumentsBinary
            new ExpressionTermTestCase()
            {
                Infix = "1 +",
                ExpectedType = TokenType.EndOfString,
            },
            //TooFewArgumentsUnary
            new ExpressionTermTestCase()
            {
                Infix = "-",
                ExpectedType = TokenType.EndOfString,
            },
            //TooFewArgumentsBinaryChained
            new ExpressionTermTestCase()
            {
                Infix = "1 + 2 *",
                ExpectedType = TokenType.EndOfString,
            },
            //MissingCloseFunctionDelimiter
            new ExpressionTermTestCase()
            {
                Infix = "max(1,2,",
                ExpectedType = TokenType.EndOfString,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(ExpressionTermTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var ex = Assert.Throws<ShuntingYard.ExpressionTermException>(() => ShuntingYard.Process(tokens, (token) => { }, (operatorInfo) => { }));

            Assert.AreEqual(testCase.ExpectedType, ex.Type);
        }

        public struct ExpressionTermTestCase
        {
            public string Infix { get; set; }
            public TokenType ExpectedType { get; set; }
        }
    }
}
