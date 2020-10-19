using ExpressionParser.Logic;
using NUnit.Framework;

namespace ExpressionParser.UTests.ShuntingYardTests
{

    [TestFixture]
    public class ArgumentMismatch
    {

        static readonly ArgumentMismatchTestCase[] s_testCases = new ArgumentMismatchTestCase[]
        {
            //TooMany
            new ArgumentMismatchTestCase()
            {
                Infix = "max(1,2,3)",
                ExpectedArugments = 2,
                ActualArguments = 3,
            },
            //TooFew
            new ArgumentMismatchTestCase()
            {
                Infix = "max(1)",
                ExpectedArugments = 2,
                ActualArguments = 1,
            },
            //TooManyNested
            new ArgumentMismatchTestCase()
            {
                Infix = "min((1 + 2),(2 + 3),(3+4))",
                ExpectedArugments = 2,
                ActualArguments = 3,
            },
            //TooFewNested
            new ArgumentMismatchTestCase()
            {
                Infix = "max((1 + 2))",
                ExpectedArugments = 2,
                ActualArguments = 1,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(ArgumentMismatchTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var ex = Assert.Throws<ShuntingYard.ArgumentMismatchException>(() => ShuntingYard.Process(tokens, (token) => { }));

            Assert.AreEqual(testCase.ExpectedArugments, ex.Expected);
            Assert.AreEqual(testCase.ActualArguments, ex.Actual);
        }

        public struct ArgumentMismatchTestCase
        {
            public string Infix { get; set; }
            public int ExpectedArugments { get; set; }
            public int ActualArguments { get; set; }
        }
    }
}
