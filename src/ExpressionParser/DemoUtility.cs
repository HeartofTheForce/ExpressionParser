using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionParser.Compilers;
using ExpressionParser.Operators;

namespace ExpressionParser
{
    public static class DemoUtility
    {
        public static readonly OperatorInfo[] OperatorMap = new OperatorInfo[]
        {
            OperatorInfo.Prefix("+"),
            OperatorInfo.Prefix("-"),
            OperatorInfo.Prefix("~"),
            OperatorInfo.Prefix("max"),
            OperatorInfo.Prefix("min"),
            OperatorInfo.Prefix("sin"),
            OperatorInfo.Prefix("cos"),
            OperatorInfo.Prefix("tan"),
            OperatorInfo.Postfix("clamp"),
            OperatorInfo.Infix("*", 0, 0),
            OperatorInfo.Infix("/", 0, 0),
            OperatorInfo.Infix("+", 1, 1),
            OperatorInfo.Infix("-", 1, 1),
            OperatorInfo.Infix("&", 2, 2),
            OperatorInfo.Infix("^", 3, 3),
            OperatorInfo.Infix("|", 4, 4),
        };

        public static readonly CompilerFunction<Expression>[] CompilerFunctions = new CompilerFunction<Expression>[]
        {
            new CompilerFunction<Expression>("+", 1, (args) => args[0]),
            new CompilerFunction<Expression>("+", 2, (args) => ReduceBinary(Expression.Add, args)),
            new CompilerFunction<Expression>("-", 1, (args) => ReduceUnary(Expression.Negate, args)),
            new CompilerFunction<Expression>("-", 2, (args) => ReduceBinary(Expression.Subtract, args)),
            new CompilerFunction<Expression>("~", 1, (args) => ReduceUnary(Expression.Not, args)),
            new CompilerFunction<Expression>("*", 2, (args) => ReduceBinary(Expression.Multiply, args)),
            new CompilerFunction<Expression>("/", 2, (args) => ReduceBinary(Expression.Divide, args)),
            new CompilerFunction<Expression>("&", 2, (args) => ReduceBinary(Expression.And, args)),
            new CompilerFunction<Expression>("^", 2, (args) => ReduceBinary(Expression.ExclusiveOr, args)),
            new CompilerFunction<Expression>("|", 2, (args) => ReduceBinary(Expression.Or, args)),
            new CompilerFunction<Expression>("max", 2, (args) => ReduceFunction(typeof(Math), nameof(Math.Max), args)),
            new CompilerFunction<Expression>("min", 2, (args) => ReduceFunction(typeof(Math), nameof(Math.Min), args)),
            new CompilerFunction<Expression>("sin", 1, (args) => ReduceFunction(typeof(Math), nameof(Math.Sin), args)),
            new CompilerFunction<Expression>("cos", 1, (args) => ReduceFunction(typeof(Math), nameof(Math.Cos), args)),
            new CompilerFunction<Expression>("tan", 1, (args) => ReduceFunction(typeof(Math), nameof(Math.Tan), args)),
            new CompilerFunction<Expression>("clamp", 3, (args) => ReduceFunction(typeof(Math), nameof(Math.Clamp), args)),
        };

        private static Expression ReduceBinary(
            Func<Expression, Expression, Expression> binaryExpression,
            Expression[] args)
        {
            if (args.Length != 2)
                throw new Exception("Expected 2 arguments");

            var left = args[0];
            var right = args[1];

            if (TypeUtility.IsFloat(left.Type) && TypeUtility.IsInteger(right.Type))
                right = Expression.Convert(right, left.Type);
            else if (TypeUtility.IsInteger(left.Type) && TypeUtility.IsFloat(right.Type))
                left = Expression.Convert(left, right.Type);

            return binaryExpression(left, right);
        }

        private static Expression ReduceUnary(
            Func<Expression, Expression> unaryExpression,
            Expression[] args)
        {
            if (args.Length != 1)
                throw new Exception("Expected 1 argument");

            return unaryExpression(args[0]);
        }

        private static Expression ReduceFunction(
            Type sourceType,
            string methodName,
            Expression[] args)
        {
            MethodInfo methodInfo;
            methodInfo = sourceType.GetMethod(methodName, args.Select(x => x.Type).ToArray());

            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < args.Length; i++)
            {
                if (TypeUtility.IsFloat(paramInfos[i].ParameterType) && TypeUtility.IsInteger(args[i].Type))
                    args[i] = Expression.Convert(args[i], paramInfos[i].ParameterType);
            }

            return Expression.Call(null, methodInfo, args);
        }
    }
}
