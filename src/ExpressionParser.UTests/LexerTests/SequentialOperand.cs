using ExpressionParser.Logic;
using NUnit.Framework;

namespace ExpressionParser.UTests.LexerTests
{

    [TestFixture]
    public class NoSequentialOperand
    {
        static readonly NoSequentialOperandTestCase[] s_testCases = new NoSequentialOperandTestCase[]
        {
            new NoSequentialOperandTestCase()
            {
                Infix = "1 1",
            },
            new NoSequentialOperandTestCase()
            {
                Infix = "a 1",
            },
            new NoSequentialOperandTestCase()
            {
                Infix = "1 a",
            },
              new NoSequentialOperandTestCase()
            {
                Infix = "a a",
            },
            new NoSequentialOperandTestCase()
            {
                Infix = ") a",
            },
            new NoSequentialOperandTestCase()
            {
                Infix = ") 1",
            },
            new NoSequentialOperandTestCase()
            {
                Infix = "1 (",
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(NoSequentialOperandTestCase testCase)
        {
            Assert.Throws<Lexer.SequentialOperandException>(() => Lexer.Process(testCase.Infix));
        }

        public struct NoSequentialOperandTestCase
        {
            public string Infix { get; set; }
        }
    }
}
