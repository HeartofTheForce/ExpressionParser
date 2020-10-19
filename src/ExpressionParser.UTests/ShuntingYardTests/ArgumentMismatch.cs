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
                ExpectedOperator = "max",
                ActualArguments = 3,
            },
            //TooFew
            new ArgumentMismatchTestCase()
            {
                Infix = "max(1)",
                ExpectedOperator = "max",
                ActualArguments = 1,
            },
            //TooManyNested
            new ArgumentMismatchTestCase()
            {
                Infix = "min((1 + 2),(2 + 3),(3+4))",
                ExpectedOperator = "min",
                ActualArguments = 3,
            },
            //TooFewNested
            new ArgumentMismatchTestCase()
            {
                Infix = "max(sin(1 + 2))",
                ExpectedOperator = "max",
                ActualArguments = 1,
            },
            //TooFewNestedFirstParameter
            new ArgumentMismatchTestCase()
            {
                Infix = "max(min(2), 1)",
                ExpectedOperator = "min",
                ActualArguments = 1,
            },
             //TooFewNestedLastParameter
            new ArgumentMismatchTestCase()
            {
                Infix = "max(1, min(2))",
                ExpectedOperator = "min",
                ActualArguments = 1,
            },
            //TooFewNestedExpressionLastParameter
            new ArgumentMismatchTestCase()
            {
                Infix = "max(1, 1 + min(2))",
                ExpectedOperator = "min",
                ActualArguments = 1,
            },
            //TooFewArgumentsBinary
            new ArgumentMismatchTestCase()
            {
                Infix = "1 +",
                ExpectedOperator = "+",
                ActualArguments = 1,
            },
            //TooFewArgumentsUnary
            new ArgumentMismatchTestCase()
            {
                Infix = "-",
                ExpectedOperator = "-",
                ActualArguments = 0,
            },
            //TooFewArgumentsBinaryChained
            new ArgumentMismatchTestCase()
            {
                Infix = "1 + 2 *",
                ExpectedOperator = "*",
                ActualArguments = 1,
            },
            //TooFewArgumentsUnaryParameter
            new ArgumentMismatchTestCase()
            {
                Infix = "max(1, -)",
                ExpectedOperator = "-",
                ActualArguments = 0,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(ArgumentMismatchTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var ex = Assert.Throws<ShuntingYard.ArgumentMismatchException>(() => ShuntingYard.Process(tokens, (token) => { }));

            Assert.AreEqual(testCase.ExpectedOperator, ex.OperatorInfo.Input);
            Assert.AreEqual(testCase.ActualArguments, ex.Actual);
            Assert.AreNotEqual(ex.OperatorInfo.ParameterCount, ex.Actual);
        }

        public struct ArgumentMismatchTestCase
        {
            public string Infix { get; set; }
            public string ExpectedOperator { get; set; }
            public int ActualArguments { get; set; }
        }
    }
}
