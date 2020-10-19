using System;
using ExpressionParser.Logic;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests.End2EndTests
{

    [TestFixture]
    public class Function
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
            //Function
            new End2EndTestCase<double>()
            {
                Infix = "sin(a) + cos(b)",
                ExpectedPostfix = "a sin b cos +",
                ExpectedFunction = (Context<double> ctx) => Math.Sin(ctx.A) + Math.Cos(ctx.B),
            },
            //FunctionExpressionParameter
            new End2EndTestCase<double>()
            {
                Infix = "tan(b + c)",
                ExpectedPostfix = "b c + tan",
                ExpectedFunction = (Context<double> ctx) => Math.Tan(ctx.B + ctx.C),
            },
            //FunctionChained
            new End2EndTestCase<double>()
            {
                Infix = "sin(cos(b))",
                ExpectedPostfix = "b cos sin",
                ExpectedFunction = (Context<double> ctx) => Math.Sin(Math.Cos(ctx.B)),
            },
            //FunctionMultiParameter
            new End2EndTestCase<double>()
            {
                Infix = "max(a,b) + min(b,c)",
                ExpectedPostfix = "a b max b c min +",
                ExpectedFunction = (Context<double> ctx) => Math.Max(ctx.A, ctx.B) + Math.Min(ctx.B, ctx.C),
            },
            //FunctionMultiExpressionParameter
            new End2EndTestCase<double>()
            {
                Infix = "max(a + b, b + c)",
                ExpectedPostfix = "a b + b c + max",
                ExpectedFunction = (Context<double> ctx) => Math.Max(ctx.A + ctx.B, ctx.B + ctx.C),
            },
            //FunctionNestedMultiExpressionParameter
            new End2EndTestCase<double>()
            {
                Infix = "max((a + b) * 2, (b / c))",
                ExpectedPostfix = "a b + 2 * b c / max",
                ExpectedFunction = (Context<double> ctx) => Math.Max((ctx.A + ctx.B) * 2, (ctx.B / ctx.C)),
            },
            //FunctionChainedMultiParameter
            new End2EndTestCase<double>()
            {
                Infix = "max(min(c, b), max(c, a))",
                ExpectedPostfix = "c b min c a max max",
                ExpectedFunction = (Context<double> ctx) => Math.Max(Math.Min(ctx.C, ctx.B), Math.Max(ctx.C, ctx.A)),
            },
            //FunctionChainedMultiParameterUnary
            new End2EndTestCase<double>()
            {
                Infix = "max(min(c, b), -a)",
                ExpectedPostfix = "c b min a u- max",
                ExpectedFunction = (Context<double> ctx) => Math.Max(Math.Min(ctx.C, ctx.B), -ctx.A),
            },
            //FunctionNestedExpresionParameter
            new End2EndTestCase<double>()
            {
                Infix = "sin(min(c, b) - a)",
                ExpectedPostfix = "c b min a - sin",
                ExpectedFunction = (Context<double> ctx) => Math.Sin(Math.Min(ctx.C, ctx.B) - ctx.A),
            },
            //PostfixInfixUnary
            new End2EndTestCase<double>()
            {
                Infix = "(a,b,c)clamp + - d",
                ExpectedPostfix = "a b c clamp d u- +",
                ExpectedFunction = (Context<double> ctx) => Math.Clamp(ctx.A, ctx.B, ctx.C) + -ctx.D,
            },
            //Postfix
            new End2EndTestCase<double>()
            {
                Infix = "(a + b, b - c, c * d)clamp",
                ExpectedPostfix = "a b + b c - c d * clamp",
                ExpectedFunction = (Context<double> ctx) => Math.Clamp(ctx.A + ctx.B, ctx.B - ctx.C, ctx.C * ctx.D),
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(End2EndTestCase<double> testCase)
        {
            var infixTokens = Lexer.Process(testCase.Infix);

            string postfixActual = PostfixCompiler.Compile(infixTokens);
            Assert.AreEqual(testCase.ExpectedPostfix, postfixActual);

            var functionActual = ExpressionCompiler.Compile<Context<double>, double>(infixTokens);
            Assert.AreEqual(testCase.ExpectedFunction(s_ctx), functionActual(s_ctx));
        }
    }
}
