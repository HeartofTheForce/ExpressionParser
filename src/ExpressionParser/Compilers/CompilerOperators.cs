using System;
using System.Linq.Expressions;
using ExpressionParser.Operators;

namespace ExpressionParser.Compilers
{
    public static class CompilerOperators
    {
        public static readonly OperatorInfo[] OperatorMap = new OperatorInfo[]
        {
            new BinaryOperator("*", "*", 5, Expression.Multiply),
            new BinaryOperator("/", "/", 5, Expression.Divide),
            new BinaryOperator("+", "+", 4, Expression.Add),
            new BinaryOperator("-", "-", 4, Expression.Subtract),
            new BinaryOperator("&", "&", 3, Expression.And),
            new BinaryOperator("^", "^", 2, Expression.ExclusiveOr),
            new BinaryOperator("|", "|", 1, Expression.Or),
            new FunctionOperator("clamp", "clamp", 3, 0, typeof(Math), nameof(Math.Clamp)),
            new UnaryOperator("+", "u+", (arg) => arg),
            new UnaryOperator("-", "u-", Expression.Negate),
            new UnaryOperator("~", "~", Expression.Not),
            new FunctionOperator("max", "max", 0, 2, typeof(Math), nameof(Math.Max)),
            new FunctionOperator("min", "min", 0, 2, typeof(Math), nameof(Math.Min)),
            new FunctionOperator("sin", "sin", 0, 1, typeof(Math), nameof(Math.Sin)),
            new FunctionOperator("cos", "cos", 0, 1, typeof(Math), nameof(Math.Cos)),
            new FunctionOperator("tan", "tan", 0, 1, typeof(Math), nameof(Math.Tan)),
        };
    }
}
