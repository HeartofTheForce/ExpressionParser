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
        public static Func<TParameter, TReturn> Compile<TParameter, TReturn>(CompilerFunction<Expression>[] compilerFunctions, Node root)
        {
            var param = Expression.Parameter(typeof(TParameter), "ctx");
            var variables = CreateVariables(param);
            Expression C(LeafNode leafNode) => CompileLeafNode(leafNode, variables);

            var compilerScope = new CompilerScope<Expression>(C, compilerFunctions);

            var expression = compilerScope.CompileNode(root);
            if (expression.Type != typeof(TReturn))
                expression = Expression.Convert(expression, typeof(TReturn));

            var func = Expression.Lambda<Func<TParameter, TReturn>>(expression, true, param).Compile();
            return func;
        }

        public static Func<TReturn> Compile<TReturn>(CompilerFunction<Expression>[] compilerFunctions, Node root)
        {
            var variables = new Dictionary<string, Expression>();
            Expression C(LeafNode leafNode) => CompileLeafNode(leafNode, variables);

            var compilerScope = new CompilerScope<Expression>(C, compilerFunctions);

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
    }
}
