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

        [Test]
        public void Parentheses()
        {
            string infix = "(a + b) * c";

            string postfixExpected = "a b + c *";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => (ctx.A + ctx.B) * ctx.C;
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void LeftPrecedence()
        {
            string infix = "a + b * c";

            string postfixExpected = "a b c * +";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => ctx.A + ctx.B * ctx.C;
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void LeftAssociative()
        {
            string infix = "a + b - c";

            string postfixExpected = "a b + c -";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => ctx.A + ctx.B - ctx.C;
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void Unary()
        {
            string infix = "-a + ~b";

            string postfixExpected = "a u- b ~ +";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => -ctx.A + ~ctx.B;
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void UnaryOutsideParentheses()
        {
            string infix = "a + ~(b + c)";

            string postfixExpected = "a b c + ~ +";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => ctx.A + ~(ctx.B + ctx.C);
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void UnaryInsideParentheses()
        {
            string infix = "(-b)";

            string postfixExpected = "b u-";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => (-ctx.B);
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void UnaryChained()
        {
            string infix = "a + - ~ b";

            string postfixExpected = "a b ~ u- +";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => ctx.A + -~ctx.B;
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void Function2Parameters()
        {
            string infix = "max a b + c";

            string postfixExpected = "a b max c +";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => Math.Max(ctx.A, ctx.B) + ctx.C;
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void Complex1()
        {
            string infix = "a+b*(~c^d-e)^(f+g*h)-i";

            string postfixExpected = "a b c ~ d e - ^ * + f g h * + i - ^";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => ctx.A + ctx.B * (~ctx.C ^ ctx.D - ctx.E) ^ (ctx.F + ctx.G * ctx.H) - ctx.I;
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        [Test]
        public void Complex2()
        {
            string infix = "(-a ^ b | c) / (d | ~e ^ +f)";

            string postfixExpected = "a u- b ^ c | d e ~ f u+ ^ | /";
            string postfixActual = Infix.Infix2Postfix(infix);
            Assert.AreEqual(postfixExpected, postfixActual);

            var functionActual = Compiler.Compile<Context, int>(postfixActual, TryParse);
            static int FunctionExpected(Context ctx) => (-ctx.A ^ ctx.B | ctx.C) / (ctx.D | ~ctx.E ^ +ctx.F);
            Assert.AreEqual(FunctionExpected(s_ctx), functionActual(s_ctx));
        }

        struct Context
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
