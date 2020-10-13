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
            var left = args[0];
            var right = args[1];

            if (TypeUtility.IsFloat(left.Type) && TypeUtility.IsInteger(right.Type))
                right = Expression.Convert(right, left.Type);
            else if (TypeUtility.IsInteger(left.Type) && TypeUtility.IsFloat(right.Type))
                left = Expression.Convert(left, right.Type);

            return BinaryExpression(left, right);

        }
    }
}
