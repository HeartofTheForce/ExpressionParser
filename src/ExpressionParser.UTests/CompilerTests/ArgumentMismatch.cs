using ExpressionParser.Compilers;
using ExpressionParser.Parsers;
using NUnit.Framework;

namespace ExpressionParser.UTests.CompilerTests
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
            //TooFewPrefix
            new ArgumentMismatchTestCase()
            {
                Infix = "max((1,2,3)clamp)",
                ExpectedOperator = "max",
                ActualArguments = 1,
            },
            //TooManyInfix
            new ArgumentMismatchTestCase()
            {
                Infix = "(1,2) + 1",
                ExpectedOperator = "+",
                ActualArguments = 3,
            },
            //TooFewPostfix
            new ArgumentMismatchTestCase()
            {
                Infix = "1 + (1,2)clamp",
                ExpectedOperator = "clamp",
                ActualArguments = 2,
            },
            //TooManyPostfix
            new ArgumentMismatchTestCase()
            {
                Infix = "(1,2,3,4)clamp",
                ExpectedOperator = "clamp",
                ActualArguments = 4,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(ArgumentMismatchTestCase testCase)
        {
            var tokens = Lexer.Process(DemoUtility.OperatorMap, testCase.Infix);
            var node = AstParser.Parse(DemoUtility.OperatorMap, tokens);

            var ex = Assert.Throws<OverloadMismatchException>(() => ExpressionCompiler.Compile<double>(DemoUtility.CompilerFunctions, node));

            Assert.AreEqual(testCase.ExpectedOperator, ex.OperatorNode.OperatorInfo.Keyword);
            Assert.AreEqual(testCase.ActualArguments, ex.OperatorNode.Children.Count);
        }

        public struct ArgumentMismatchTestCase
        {
            public string Infix { get; set; }
            public string ExpectedOperator { get; set; }
            public int ActualArguments { get; set; }
        }
    }
}
