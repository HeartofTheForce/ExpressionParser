using System;
using System.Linq.Expressions;

namespace ExpressionParser.Logic.Operators
{
    public class UnaryOperator : OperatorInfo<Expression>
    {
        public Func<Expression, Expression> UnaryExpression { get; }

        public UnaryOperator(string input, string output, Func<Expression, Expression> unaryExpression)
        : base(input, output, int.MaxValue, Associativity.Right, 0, 1)
        {
            UnaryExpression = unaryExpression;
        }

        override public Expression Reduce(Expression[] args)
        {
            return UnaryExpression(args[0]);
        }
    }
}
