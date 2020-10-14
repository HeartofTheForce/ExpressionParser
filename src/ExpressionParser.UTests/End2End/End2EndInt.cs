using System;
using System.Linq;
using ExpressionParser.Logic;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests.End2End
{

    [TestFixture]
    public class End2EndInt
    {
        static readonly Context<int> s_intCtx = new Context<int>()
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

        static readonly End2EndTestCase<int>[] s_intTestCases = new End2EndTestCase<int>[]
        {
            //Parentheses
            new End2EndTestCase<int>()
            {
                Infix = "(a + b) * c",
                ExpectedPostfix = "a b + c *",
                ExpectedFunction = (Context<int> ctx) => (ctx.A + ctx.B) * ctx.C,
            },
            //LeftPrecedence
            new End2EndTestCase<int>()
            {
                Infix = "a + b * c",
                ExpectedPostfix = "a b c * +",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ctx.B * ctx.C,
            },
            //LeftAssociative
            new End2EndTestCase<int>()
            {
                Infix = "a + b - c",
                ExpectedPostfix = "a b + c -",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ctx.B - ctx.C,
            },
            //Unary
            new End2EndTestCase<int>()
            {
                Infix = "-a + ~b",
                ExpectedPostfix = "a u- b ~ +",
                ExpectedFunction = (Context<int> ctx) => -ctx.A + ~ctx.B,
            },
            //UnaryOutsideParentheses
            new End2EndTestCase<int>()
            {
                Infix = "a + ~(b + c)",
                ExpectedPostfix = "a b c + ~ +",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ~(ctx.B + ctx.C),
            },
            //UnaryInsideParentheses
            new End2EndTestCase<int>()
            {
                Infix = "(-b)",
                ExpectedPostfix = "b u-",
                ExpectedFunction = (Context<int> ctx) => (-ctx.B),
            },
            //UnaryChained
            new End2EndTestCase<int>()
            {
                Infix = "a + - ~ b",
                ExpectedPostfix = "a b ~ u- +",
                ExpectedFunction = (Context<int> ctx) => ctx.A + -~ctx.B,
            },
            //Function2Parameters
            new End2EndTestCase<int>()
            {
                Infix = "max(a b) + c",
                ExpectedPostfix = "a b max c +",
                ExpectedFunction = (Context<int> ctx) => Math.Max(ctx.A, ctx.B) + ctx.C,
            },
            //Complex1
            new End2EndTestCase<int>()
            {
                Infix = "a+b*(~c^d-e)^(f+g*h)-i",
                ExpectedPostfix = "a b c ~ d e - ^ * + f g h * + i - ^",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ctx.B * (~ctx.C ^ ctx.D - ctx.E) ^ (ctx.F + ctx.G * ctx.H) - ctx.I,
            },
            //Complex2
            new End2EndTestCase<int>()
            {
                Infix = "(-a ^ b | c) / (d | ~e ^ +f)",
                ExpectedPostfix = "a u- b ^ c | d e ~ f u+ ^ | /",
                ExpectedFunction = (Context<int> ctx) => (-ctx.A ^ ctx.B | ctx.C) / (ctx.D | ~ctx.E ^ +ctx.F),
            },
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

        [TestCaseSource(nameof(s_intTestCases))]
        public void IntTestCases(End2EndTestCase<int> testCase)
        {
            var infixTokens = Lexer.Process(testCase.Infix);

            string postfixActual = PostfixCompiler.Compile(infixTokens);
            Assert.AreEqual(testCase.ExpectedPostfix, postfixActual);

            var functionActual = ExpressionCompiler.Compile<Context<int>, int>(infixTokens);
            Assert.AreEqual(testCase.ExpectedFunction(s_intCtx), functionActual(s_intCtx));
        }
    }
}
