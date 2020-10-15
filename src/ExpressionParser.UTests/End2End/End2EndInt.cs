using System;
using ExpressionParser.Logic;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests.End2End
{

    [TestFixture]
    public class End2EndInt
    {
        static readonly Context<int> s_ctx = new Context<int>()
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4,
            E = 5,
            F = 6,
            G = 7,
            H = 8,
            I = 9,
        };

        static readonly End2EndTestCase<int>[] s_testCases = new End2EndTestCase<int>[]
        {
            //ReturnFloat2Int
            new End2EndTestCase<int>()
            {
                Infix = "2.5 + 3.3",
                ExpectedPostfix = "2.5 3.3 +",
                ExpectedFunction = (Context<int> ctx) => (int)(2.5 + 3.3),
            },
            //IntOnlyBitwise
            new End2EndTestCase<int>()
            {
                Infix = "~(1 | 4)",
                ExpectedPostfix = "1 4 | ~",
                ExpectedFunction = (Context<int> ctx) => ~(1 | 4),
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void IntTestCases(End2EndTestCase<int> testCase)
        {
            var infixTokens = Lexer.Process(testCase.Infix);

            string postfixActual = PostfixCompiler.Compile(infixTokens);
            Assert.AreEqual(testCase.ExpectedPostfix, postfixActual);

            var functionActual = ExpressionCompiler.Compile<Context<int>, int>(infixTokens);
            Assert.AreEqual(testCase.ExpectedFunction(s_ctx), functionActual(s_ctx));
        }
    }
}
