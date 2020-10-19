using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionParser.Logic.Operators
{
    public class FunctionOperator : OperatorInfo<Expression>
    {
        public Type SourceType { get; }
        public string MethodName { get; }

        public FunctionOperator(string input, string output, int preArgCount, int postArgCount, Type sourceType, string methodName)
        : base(input, output, int.MaxValue, Associativity.Right, preArgCount, postArgCount)
        {
            SourceType = sourceType;
            MethodName = methodName;
        }

        override public Expression Reduce(Expression[] args)
        {
            MethodInfo methodInfo;
            methodInfo = SourceType.GetMethod(MethodName, args.Select(x => x.Type).ToArray());

            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < ParameterCount; i++)
            {
                if (TypeUtility.IsFloat(paramInfos[i].ParameterType) && TypeUtility.IsInteger(args[i].Type))
                    args[i] = Expression.Convert(args[i], paramInfos[i].ParameterType);
            }

            return Expression.Call(null, methodInfo, args);
        }
    }
}
