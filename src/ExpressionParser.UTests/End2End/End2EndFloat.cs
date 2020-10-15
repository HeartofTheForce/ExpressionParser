using System;
using ExpressionParser.Logic;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests.End2End
{

    [TestFixture]
    public class End2EndFloat
    {
        static readonly Context<double> s_ctx = new Context<double>()
        {
            A = 1.1,
            B = 2.2,
            C = 3.3,
            D = 4.4,
            E = 5.5,
            F = 6.6,
            G = 7.7,
            H = 8.8,
            I = 9.9,
        };

        static readonly End2EndTestCase<double>[] s_testCases = new End2EndTestCase<double>[]
        {
            //LeftToFloat
            new End2EndTestCase<double>()
            {
                Infix = "2 + 1.5",
                ExpectedPostfix = "2 1.5 +",
                ExpectedFunction = (Context<double> ctx) => 2 + 1.5,
            },
            //RightToFloat
            new End2EndTestCase<double>()
            {
                Infix = "1.5 + 2",
                ExpectedPostfix = "1.5 2 +",
                ExpectedFunction = (Context<double> ctx) => 1.5 + 2,
            },
            //SinCosTan
            new End2EndTestCase<double>()
            {
                Infix = "sin(1.0) + cos(1.0) + tan(1.0)",
                ExpectedPostfix = "1.0 sin 1.0 cos + 1.0 tan +",
                ExpectedFunction = (Context<double> ctx) => Math.Sin(1.0) + Math.Cos(1.0) + Math.Tan(1.0),
            },
            //MaxIntFloat
            new End2EndTestCase<double>()
            {
                Infix = "max(2, b)",
                ExpectedPostfix = "2 b max",
                ExpectedFunction = (Context<double> ctx) => Math.Max(2, ctx.B),
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void FloatTestCases(End2EndTestCase<double> testCase)
        {
            var infixTokens = Lexer.Process(testCase.Infix);

            string postfixActual = PostfixCompiler.Compile(infixTokens);
            Assert.AreEqual(testCase.ExpectedPostfix, postfixActual);

            var functionActual = ExpressionCompiler.Compile<Context<double>, double>(infixTokens);
            Assert.AreEqual(testCase.ExpectedFunction(s_ctx), functionActual(s_ctx));
        }
    }
}
