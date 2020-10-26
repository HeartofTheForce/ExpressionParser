using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionParser.Operators;
using ExpressionParser.Parsers;

namespace ExpressionParser.Compilers
{
    public static class ExpressionCompiler
    {
        public static Func<TParameter, TReturn> Compile<TParameter, TReturn>(IEnumerable<Token> infix)
        {
            var param = Expression.Parameter(typeof(TParameter), "ctx");
            var values = new Stack<Expression>();
            ShuntingYard.Process(infix, (value) => ProcessOperand(value, param, values), (operatorInfo) => ProcessOperator(operatorInfo, values));

            if (values.Count != 1)
                throw new Exception($"Expression not fully reduced, remaining values: {values.Count}");

            var expression = values.Peek();
            if (expression.Type != typeof(TReturn))
                expression = Expression.Convert(expression, typeof(TReturn));

            var func = Expression.Lambda<Func<TParameter, TReturn>>(expression, true, param).Compile();
            return func;
        }

        public static void ProcessOperator(OperatorInfo operatorInfo, Stack<Expression> values)
        {
            if (operatorInfo.ArgCount > values.Count)
                throw new Exception($"Not enough arguments: {operatorInfo.Output}");

            var buffer = new Expression[operatorInfo.ArgCount];
            for (int j = operatorInfo.ArgCount - 1; j >= 0; j--)
            {
                buffer[j] = values.Pop();
            }

            if (operatorInfo is IReducible<Expression> reducible)
                values.Push(reducible.Reduce(buffer));
            else
                throw new Exception($"Cannot reduce: {operatorInfo.Output}");
        }

        public static void ProcessOperand(string value, ParameterExpression param, Stack<Expression> values)
        {
            Expression operandExpression;
            if (TryGetMemberInfo(param, value, out var memberInfo))
                operandExpression = Expression.MakeMemberAccess(param, memberInfo);
            else if (int.TryParse(value, out int intValue))
                operandExpression = Expression.Constant(intValue);
            else if (double.TryParse(value, out double doubleValue))
                operandExpression = Expression.Constant(doubleValue);
            else
                throw new Exception($"Cannot parse operand: {value}");

            values.Push(operandExpression);
        }

        private static bool TryGetMemberInfo(ParameterExpression param, string propertyOrFieldName, out MemberInfo memberInfo)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy;

            var pi = param.Type.GetProperty(propertyOrFieldName, bindingFlags);
            if (pi != null)
            {
                memberInfo = pi;
                return true;
            }

            var fi = param.Type.GetField(propertyOrFieldName, bindingFlags);
            if (fi != null)
            {
                memberInfo = fi;
                return true;
            }

            memberInfo = null;
            return false;
        }
    }
}

