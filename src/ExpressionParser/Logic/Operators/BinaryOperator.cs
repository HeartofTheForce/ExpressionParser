using System;
using System.Linq.Expressions;

namespace ExpressionParser.Logic.Operators
{
    public class BinaryOperator : IOperatorInfo<Expression>
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Precedence { get; set; }
        public Associativity Associativity => Associativity.Left;
        public int ParameterCount => 2;
        public Func<Expression, Expression, Expression> BinaryExpression { get; set; }

        Expression IOperatorInfo<Expression>.Reduce(Expression[] args)
        {
            static bool IsFloat(Type type) => type == typeof(float) || type == typeof(double) || type == typeof(decimal);
            static bool IsInteger(Type type) =>
                    type == typeof(sbyte) || type == typeof(byte) ||
                    type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                    type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);

            var left = args[0];
            var right = args[1];

            if (IsFloat(left.Type) && IsInteger(right.Type))
                right = Expression.Convert(right, left.Type);
            else if (IsInteger(left.Type) && IsFloat(right.Type))
                left = Expression.Convert(left, right.Type);

            return BinaryExpression(left, right);

        }
    }
}
