using System;
using System.Linq.Expressions;

namespace ExpressionParser.Operators
{
    public class UnaryOperator : OperatorInfo, IReducible<Expression>
    {
        public Func<Expression, Expression> UnaryExpression { get; }

        public UnaryOperator(string input, string output, Func<Expression, Expression> unaryExpression)
        : base(input, output, int.MaxValue, Associativity.Right, 0, 1)
        {
            UnaryExpression = unaryExpression;
        }

        public Expression Reduce(Expression[] args)
        {
            return UnaryExpression(args[0]);
        }
    }
}
