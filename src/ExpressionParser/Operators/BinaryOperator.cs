using System;
using System.Linq.Expressions;

namespace ExpressionParser.Operators
{
    public class BinaryOperator : OperatorInfo, IReducible<Expression>
    {
        public Func<Expression, Expression, Expression> BinaryExpression { get; set; }

        public BinaryOperator(string input, string output, int precedence, Func<Expression, Expression, Expression> binaryExpression)
        : base(input, output, precedence, Associativity.Left, 1, 1)
        {
            BinaryExpression = binaryExpression;
        }

        public Expression Reduce(Expression[] args)
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
