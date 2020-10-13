using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionParser.Logic.Operators
{
    public class MethodCallOperator : IOperatorInfo<Expression>
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Precedence => int.MaxValue;
        public Associativity Associativity => Associativity.Right;
        public int ParameterCount { get; set; }

        public Type SourceType { get; set; }
        public string MethodName { get; set; }

        Expression IOperatorInfo<Expression>.Reduce(Expression[] args)
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
