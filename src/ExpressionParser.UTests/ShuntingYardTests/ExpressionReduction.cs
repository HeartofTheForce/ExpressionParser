using ExpressionParser.Logic;
using NUnit.Framework;

namespace ExpressionParser.UTests.ShuntingYardTests
{

    [TestFixture]
    public class ExpressionReduction
    {

        static readonly ExpressionReductionTestCase[] s_testCases = new ExpressionReductionTestCase[]
        {
            //EmptyDelimiterClose
            new ExpressionReductionTestCase()
            {
                Infix = "max(1,2,)",
                ExpectedRemainingValues = 0,
            },
            //EmptyDelimiterDelimiter
            new ExpressionReductionTestCase()
            {
                Infix = "max(1,,3)",
                ExpectedRemainingValues = 0,
            },
            //EmptyOpenDelimiter
            new ExpressionReductionTestCase()
            {
                Infix = "max(,2,3)",
                ExpectedRemainingValues = 0,
            },
            //EmptyParentheses
            new ExpressionReductionTestCase()
            {
                Infix = "()",
                ExpectedRemainingValues = 0,
            },
            //TooFewArgumentsBinary
            new ExpressionReductionTestCase()
            {
                Infix = "1 +",
                ExpectedRemainingValues = 0,
            },
            //TooFewArgumentsUnary
            new ExpressionReductionTestCase()
            {
                Infix = "-",
                ExpectedRemainingValues = 0,
            },
            //TooFewArgumentsBinaryParameter
            new ExpressionReductionTestCase()
            {
                Infix = "max(1 + 2 * ,3)",
                ExpectedRemainingValues = 0,
            },
            //TooFewArgumentsUnaryParameter
            new ExpressionReductionTestCase()
            {
                Infix = "max(1, -)",
                ExpectedRemainingValues = 0,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(ExpressionReductionTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var ex = Assert.Throws<ShuntingYard.ExpressionReductionException>(() => ShuntingYard.Process(tokens, (token) => { }));

            Assert.AreEqual(testCase.ExpectedRemainingValues, ex.RemainingValues);
        }

        public struct ExpressionReductionTestCase
        {
            public string Infix { get; set; }
            public int ExpectedRemainingValues { get; set; }
        }
    }
}
