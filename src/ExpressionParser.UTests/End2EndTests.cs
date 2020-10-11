using System;
using ExpressionParser.Parser;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests
{

    [TestFixture]
    public class End2EndTests
    {
        static readonly Context s_ctx = new Context()
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

        static readonly End2EndTestCase[] TestCases = new End2EndTestCase[]
        {
            //Parentheses
            new End2EndTestCase()
            {
                Infix = "(a + b) * c",
                ExpectedPostfix = "a b + c *",
                ExpectedFunction = (Context ctx) => (ctx.A + ctx.B) * ctx.C,
            },
            //LeftPrecedence
            new End2EndTestCase()
            {
                Infix = "a + b * c",
                ExpectedPostfix = "a b c * +",
                ExpectedFunction = (Context ctx) => ctx.A + ctx.B * ctx.C,
            },
            //LeftAssociative
            new End2EndTestCase()
            {
                Infix = "a + b - c",
                ExpectedPostfix = "a b + c -",
                ExpectedFunction = (Context ctx) => ctx.A + ctx.B - ctx.C,
            },
            //Unary
            new End2EndTestCase()
            {
                Infix = "-a + ~b",
                ExpectedPostfix = "a u- b ~ +",
                ExpectedFunction = (Context ctx) => -ctx.A + ~ctx.B,
            },
            //UnaryOutsideParentheses
            new End2EndTestCase()
            {
                Infix = "a + ~(b + c)",
                ExpectedPostfix = "a b c + ~ +",
                ExpectedFunction = (Context ctx) => ctx.A + ~(ctx.B + ctx.C),
            },
            //UnaryInsideParentheses
            new End2EndTestCase()
            {
                Infix = "(-b)",
                ExpectedPostfix = "b u-",
                ExpectedFunction = (Context ctx) => (-ctx.B),
            },
            //UnaryChained
            new End2EndTestCase()
            {
                Infix = "a + - ~ b",
                ExpectedPostfix = "a b ~ u- +",
                ExpectedFunction = (Context ctx) => ctx.A + -~ctx.B,
            },
            //Function2Parameters
            new End2EndTestCase()
            {
                Infix = "max a b + c",
                ExpectedPostfix = "a b max c +",
                ExpectedFunction = (Context ctx) => Math.Max(ctx.A, ctx.B) + ctx.C,
            },
            //Complex1
            new End2EndTestCase()
            {
                Infix = "a+b*(~c^d-e)^(f+g*h)-i",
                ExpectedPostfix = "a b c ~ d e - ^ * + f g h * + i - ^",
                ExpectedFunction = (Context ctx) => ctx.A + ctx.B * (~ctx.C ^ ctx.D - ctx.E) ^ (ctx.F + ctx.G * ctx.H) - ctx.I,
            },
            //Complex2
            new End2EndTestCase()
            {
                Infix = "(-a ^ b | c) / (d | ~e ^ +f)",
                ExpectedPostfix = "a u- b ^ c | d e ~ f u+ ^ | /",
                ExpectedFunction = (Context ctx) => (-ctx.A ^ ctx.B | ctx.C) / (ctx.D | ~ctx.E ^ +ctx.F),
            },
        };

        [TestCaseSource(nameof(TestCases))]
        public void End2End(End2EndTestCase testCase)
        {
            string postfixActual = Infix.Infix2Postfix(testCase.Infix);
            Assert.AreEqual(testCase.ExpectedPostfix, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            Assert.AreEqual(testCase.ExpectedFunction(s_ctx), functionActual(s_ctx));
        }

        public struct End2EndTestCase
        {
            public string Infix { get; set; }
            public string ExpectedPostfix { get; set; }
            public Func<Context, int> ExpectedFunction { get; set; }
        }

        public struct Context
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
            public int D { get; set; }
            public int E { get; set; }
            public int F { get; set; }
            public int G { get; set; }
            public int H { get; set; }
            public int I { get; set; }
        }

        static bool TryParse(string target, out int value)
        {
            return int.TryParse(target, out value);
        }
    }
}
