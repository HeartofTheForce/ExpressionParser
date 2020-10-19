using System;
using System.Linq.Expressions;
using ExpressionParser.Logic.Operators;

namespace ExpressionParser.Logic
{
    public static class CompilerOperators
    {
        public static readonly OperatorInfo<Expression>[] InfixOperatorMap = new BinaryOperator[]
        {
            new BinaryOperator("*", "*", 5, Expression.Multiply),
            new BinaryOperator("/", "/", 5, Expression.Divide),
            new BinaryOperator("+", "+", 4, Expression.Add),
            new BinaryOperator("-", "-", 4, Expression.Subtract),
            new BinaryOperator("&", "&", 3, Expression.And),
            new BinaryOperator("^", "^", 2, Expression.ExclusiveOr),
            new BinaryOperator("|", "|", 1, Expression.Or),
        };

        public static readonly OperatorInfo<Expression>[] PrefixOperatorMap = new OperatorInfo<Expression>[]
        {
            new UnaryOperator("+", "u+", (arg) => arg),
            new UnaryOperator("-", "u-", Expression.Negate),
            new UnaryOperator("~", "~", Expression.Not),
            new FunctionOperator("max", "max", 2, typeof(Math), nameof(Math.Max)),
            new FunctionOperator("min", "min", 2, typeof(Math), nameof(Math.Min)),
            new FunctionOperator("sin", "sin", 1, typeof(Math), nameof(Math.Sin)),
            new FunctionOperator("cos", "cos", 1, typeof(Math), nameof(Math.Cos)),
            new FunctionOperator("tan", "tan", 1, typeof(Math), nameof(Math.Tan)),
        };
    }
}

