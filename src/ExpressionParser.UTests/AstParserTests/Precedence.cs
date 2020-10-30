using ExpressionParser.Operators;
using ExpressionParser.Parsers;
using NUnit.Framework;

namespace ExpressionParser.UTests.AstParserTests
{

    [TestFixture]
    public class Precedence
    {
        private const int High = 0;
        private const int Equal = 1;
        private const int Low = 2;

        private static readonly OperatorInfo[] s_testOperatorMap = new OperatorInfo[]
        {
            OperatorInfo.Prefix("pre0"),
            OperatorInfo.Prefix("pre1"),
            OperatorInfo.Postfix("post0"),
            OperatorInfo.Postfix("post1"),
            OperatorInfo.Infix("lowEqIn", Low, Equal),
            OperatorInfo.Infix("eqLowIn", Equal, Low),
            OperatorInfo.Infix("highEqIn", High, Equal),
            OperatorInfo.Infix("eqHighIn", Equal, High),
        };

        static readonly PrecedenceTestCase[] s_testCases = new PrecedenceTestCase[]
        {
            //Prefix right associative
            new PrecedenceTestCase()
            {
                Infix = "pre0 pre1 1",
                ExpectedNodeString = "(pre0 (pre1 1))",
            },
            //Postfix left associative
            new PrecedenceTestCase()
            {
                Infix = "1 post0 post1",
                ExpectedNodeString = "(post1 (post0 1))",
            },
            //Infix left associative
            new PrecedenceTestCase()
            {
                Infix = "1 lowEqIn 2 eqLowIn 3",
                ExpectedNodeString = "(eqLowIn (lowEqIn 1 2) 3)",
            },
            //Infix precedence
            new PrecedenceTestCase()
            {
                Infix = "1 eqLowIn 2 highEqIn 3",
                ExpectedNodeString = "(eqLowIn 1 (highEqIn 2 3))",
            },
            //Prefix-Postfix precedence
            new PrecedenceTestCase()
            {
                Infix = "pre0 1 post0",
                ExpectedNodeString = "(pre0 (post0 1))",
            },
            //Infix-Prefix precedence
            new PrecedenceTestCase()
            {
                Infix = "pre0 1 highEqIn 2",
                ExpectedNodeString = "(highEqIn (pre0 1) 2)",
            },
            //Infix-Prefix precedence
            new PrecedenceTestCase()
            {
                Infix = "1 eqHighIn pre0 2",
                ExpectedNodeString = "(eqHighIn 1 (pre0 2))",
            },
            //Infix-Postfix precedence
            new PrecedenceTestCase()
            {
                Infix = "1 post0 highEqIn 2",
                ExpectedNodeString = "(highEqIn (post0 1) 2)",
            },
            //Infix-Postfix precedence
            new PrecedenceTestCase()
            {
                Infix = "1 eqHighIn 2 post0",
                ExpectedNodeString = "(eqHighIn 1 (post0 2))",
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(PrecedenceTestCase testCase)
        {
            var tokens = Lexer.Process(s_testOperatorMap, testCase.Infix);

            var node = AstParser.Parse(s_testOperatorMap, tokens);
            Assert.AreEqual(testCase.ExpectedNodeString, node.ToString());
        }

        public struct PrecedenceTestCase
        {
            public string Infix { get; set; }
            public string ExpectedNodeString { get; set; }
        }
    }
}
