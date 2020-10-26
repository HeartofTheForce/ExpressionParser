using ExpressionParser.Parsers;
using NUnit.Framework;

namespace ExpressionParser.UTests.ShuntingYardTests
{
    [TestFixture]
    public class ExpressionReduction
    {
        static readonly ExpressionReductionTestCase[] s_testCases = new ExpressionReductionTestCase[]
        {
            //ConstantConstantConstant
            new ExpressionReductionTestCase()
            {
                Infix = "(1,2,3)",
                ExpectedRemaining = 3,
            },
            //FunctionConstant
            new ExpressionReductionTestCase()
            {
                Infix = "(sin(1.0), 2)",
                ExpectedRemaining = 2,
            },
            //ConstantFunction
            new ExpressionReductionTestCase()
            {
                Infix = "(1, max(2,3))",
                ExpectedRemaining = 2,
            },
            //ParameterConstantConstantConstant
            new ExpressionReductionTestCase()
            {
                Infix = "max(1,(2,3,4))",
                ExpectedRemaining = 3,
            },
            //ParameterConstantConstantConstant
            new ExpressionReductionTestCase()
            {
                Infix = "max((1,2,3), 4)",
                ExpectedRemaining = 3,
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(ExpressionReductionTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var ex = Assert.Throws<ShuntingYard.ExpressionReductionException>(() => ShuntingYard.Process(tokens, (value) => { }, (operatorInfo) => { }));

            Assert.AreEqual(testCase.ExpectedRemaining, ex.RemainingValues);
        }

        public struct ExpressionReductionTestCase
        {
            public string Infix { get; set; }
            public int ExpectedRemaining { get; set; }
        }
    }
}
