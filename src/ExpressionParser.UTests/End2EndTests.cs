using System;
using System.Linq;
using ExpressionParser.Logic;
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

        static readonly End2EndTestCase[] s_testCases = new End2EndTestCase[]
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
                Infix = "max(a b) + c",
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
            //ReturnFloat2Int
            new End2EndTestCase()
            {
                Infix = "2.5 + 3.3",
                ExpectedPostfix = "2.5 3.3 +",
                ExpectedFunction = (Context ctx) => (int)(2.5f + 3.3f),
            },
            //LeftToFloat
            new End2EndTestCase()
            {
                Infix = "2 + 1.5",
                ExpectedPostfix = "2 1.5 +",
                ExpectedFunction = (Context ctx) => (int)(2 + 1.5f),
            },
             //RightToFloat
            new End2EndTestCase()
            {
                Infix = "1.5 + 2",
                ExpectedPostfix = "1.5 2 +",
                ExpectedFunction = (Context ctx) => (int)(1.5f + 2),
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(End2EndTestCase testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);
            var postfixActual = Infix.Infix2Postfix(tokens);

            string postfixActualString = string.Join(' ', postfixActual.Select(x => x.Value));
            Assert.AreEqual(testCase.ExpectedPostfix, postfixActualString);

            var functionActual = Compiler.Compile<Context, int>(postfixActual);
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
    }
}
