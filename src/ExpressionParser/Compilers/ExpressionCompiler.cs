using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionParser.Parsers;

namespace ExpressionParser.Compilers
{
    public static class ExpressionCompiler
    {
        private static readonly CompilerFunction<Expression>[] s_compilerFunctions = new CompilerFunction<Expression>[]
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

        public static Func<TParameter, TReturn> Compile<TParameter, TReturn>(Node root)
        {
            var param = Expression.Parameter(typeof(TParameter), "ctx");
            var variables = CreateVariables(param);
            Expression C(LeafNode leafNode) => CompileLeafNode(leafNode, variables);

            var compilerScope = new CompilerScope<Expression>(C, s_compilerFunctions);

            var expression = compilerScope.CompileNode(root);
            if (expression.Type != typeof(TReturn))
                expression = Expression.Convert(expression, typeof(TReturn));

            var func = Expression.Lambda<Func<TParameter, TReturn>>(expression, true, param).Compile();
            return func;
        }

        public static Func<TReturn> Compile<TReturn>(Node root)
        {
            var variables = new Dictionary<string, Expression>();
            Expression C(LeafNode leafNode) => CompileLeafNode(leafNode, variables);

            var compilerScope = new CompilerScope<Expression>(C, s_compilerFunctions);

            var expression = compilerScope.CompileNode(root);
            if (expression.Type != typeof(TReturn))
                expression = Expression.Convert(expression, typeof(TReturn));

            var func = Expression.Lambda<Func<TReturn>>(expression, true).Compile();
            return func;
        }

        private static Expression CompileLeafNode(LeafNode leafNode, Dictionary<string, Expression> variables)
        {
            if (variables.TryGetValue(leafNode.Value, out var variable))
                return variable;
            else if (int.TryParse(leafNode.Value, out int intValue))
                return Expression.Constant(intValue);
            else if (double.TryParse(leafNode.Value, out double doubleValue))
                return Expression.Constant(doubleValue);
            else
                throw new Exception($"Cannot compile {nameof(LeafNode)} '{leafNode}'");
        }

        private static Dictionary<string, Expression> CreateVariables(ParameterExpression param)
        {
            var variables = new Dictionary<string, Expression>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var properties = param.Type.GetProperties(bindingFlags);
            for (int i = 0; i < properties.Length; i++)
            {
                variables[properties[i].Name.ToLower()] = Expression.MakeMemberAccess(param, properties[i]);
            }

            var fields = param.Type.GetFields(bindingFlags);
            for (int i = 0; i < fields.Length; i++)
            {
                variables[fields[i].Name.ToLower()] = Expression.MakeMemberAccess(param, fields[i]);
            }

            return variables;
        }


        public static Expression ReduceBinary(
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

        public static Expression ReduceUnary(
            Func<Expression, Expression> unaryExpression,
            Expression[] args)
        {
            if (args.Length != 1)
                throw new Exception("Expected 1 argument");

            return unaryExpression(args[0]);
        }

        public static Expression ReduceFunction(
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
