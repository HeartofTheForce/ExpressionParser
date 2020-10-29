using ExpressionParser.Compilers;
using ExpressionParser.Parsers;
using NUnit.Framework;
#pragma warning disable IDE0047

namespace ExpressionParser.UTests.End2EndTests
{
    [TestFixture]
    public class OperatorPrecedence
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
            //Parentheses
            new End2EndTestCase<int>()
            {
                Infix = "(a + b) * c",
                ExpectedNodeString = "(* (+ a b) c)",
                ExpectedFunction = (Context<int> ctx) => (ctx.A + ctx.B) * ctx.C,
            },
            //LeftPrecedence
            new End2EndTestCase<int>()
            {
                Infix = "a + b * c",
                ExpectedNodeString = "(+ a (* b c))",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ctx.B * ctx.C,
            },
            //LeftAssociative
            new End2EndTestCase<int>()
            {
                Infix = "a + b - c",
                ExpectedNodeString = "(- (+ a b) c)",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ctx.B - ctx.C,
            },
            //Prefix
            new End2EndTestCase<int>()
            {
                Infix = "-a + ~b",
                ExpectedNodeString = "(+ (- a) (~ b))",
                ExpectedFunction = (Context<int> ctx) => -ctx.A + ~ctx.B,
            },
            //PrefixOutsideParentheses
            new End2EndTestCase<int>()
            {
                Infix = "a + ~(b + c)",
                ExpectedNodeString = "(+ a (~ (+ b c)))",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ~(ctx.B + ctx.C),
            },
            //PrefixInsideParentheses
            new End2EndTestCase<int>()
            {
                Infix = "(-b)",
                ExpectedNodeString = "(- b)",
                ExpectedFunction = (Context<int> ctx) => (-ctx.B),
            },
            //PrefixChained
            new End2EndTestCase<int>()
            {
                Infix = "a + - ~ b",
                ExpectedNodeString = "(+ a (- (~ b)))",
                ExpectedFunction = (Context<int> ctx) => ctx.A + -~ctx.B,
            },
            //Complex1
            new End2EndTestCase<int>()
            {
                Infix = "a+b*(~c^d-e)^(f+g*h)-i",
                ExpectedNodeString = "(^ (+ a (* b (^ (~ c) (- d e)))) (- (+ f (* g h)) i))",
                ExpectedFunction = (Context<int> ctx) => ctx.A + ctx.B * (~ctx.C ^ ctx.D - ctx.E) ^ (ctx.F + ctx.G * ctx.H) - ctx.I,
            },
            //Complex2
            new End2EndTestCase<int>()
            {
                Infix = "(-a ^ b | c) / (d | ~e ^ +f)",
                ExpectedNodeString = "(/ (| (^ (- a) b) c) (| d (^ (~ e) (+ f))))",
                ExpectedFunction = (Context<int> ctx) => (-ctx.A ^ ctx.B | ctx.C) / (ctx.D | ~ctx.E ^ +ctx.F),
            },
        };

        [TestCaseSource(nameof(s_testCases))]
        public void TestCases(End2EndTestCase<int> testCase)
        {
            var tokens = Lexer.Process(testCase.Infix);

            var node = AstParser.Parse(tokens);
            Assert.AreEqual(testCase.ExpectedNodeString, node.ToString());

            var functionActual = ExpressionCompiler.Compile<Context<int>, int>(node);
            Assert.AreEqual(testCase.ExpectedFunction(s_ctx), functionActual(s_ctx));
        }
    }
}
