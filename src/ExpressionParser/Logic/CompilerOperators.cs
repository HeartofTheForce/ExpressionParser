using System;
using System.Linq.Expressions;
using ExpressionParser.Logic.Operators;

namespace ExpressionParser.Logic
{
    public static class CompilerOperators
    {
        public static readonly IOperatorInfo<Expression>[] BinaryOperatorMap = new BinaryOperator[]
        {
            new BinaryOperator() { Input = "*", Output = "*", Precedence = 5, BinaryExpression = Expression.Multiply},
            new BinaryOperator() { Input = "/", Output = "/", Precedence = 5, BinaryExpression = Expression.Divide},
            new BinaryOperator() { Input = "+", Output = "+", Precedence = 4, BinaryExpression = Expression.Add},
            new BinaryOperator() { Input = "-", Output = "-", Precedence = 4, BinaryExpression = Expression.Subtract},
            new BinaryOperator() { Input = "&", Output = "&", Precedence = 3, BinaryExpression = Expression.And},
            new BinaryOperator() { Input = "^", Output = "^", Precedence = 2, BinaryExpression = Expression.ExclusiveOr},
            new BinaryOperator() { Input = "|", Output = "|", Precedence = 1, BinaryExpression = Expression.Or},
        };

        public static readonly IOperatorInfo<Expression>[] FunctionOperatorMap = new IOperatorInfo<Expression>[]
        {
            new UnaryOperator() { Input = "+", Output = "u+", UnaryExpression = (arg) => arg },
            new UnaryOperator() { Input = "-", Output = "u-", UnaryExpression = Expression.Negate },
            new UnaryOperator() { Input = "~", Output = "~", UnaryExpression = Expression.Not },
            new MethodCallOperator() { Input = "max", Output = "max", ParameterCount = 2, SourceType = typeof(Math), MethodName = nameof(Math.Max) },
            new MethodCallOperator() { Input = "min", Output = "min", ParameterCount = 2, SourceType = typeof(Math), MethodName = nameof(Math.Min) },
            new MethodCallOperator() { Input = "sin", Output = "sin", ParameterCount = 1, SourceType = typeof(Math), MethodName = nameof(Math.Sin) },
            new MethodCallOperator() { Input = "cos", Output = "cos", ParameterCount = 1, SourceType = typeof(Math), MethodName = nameof(Math.Cos) },
            new MethodCallOperator() { Input = "tan", Output = "tan", ParameterCount = 1, SourceType = typeof(Math), MethodName = nameof(Math.Tan) },
        };
    }
}

