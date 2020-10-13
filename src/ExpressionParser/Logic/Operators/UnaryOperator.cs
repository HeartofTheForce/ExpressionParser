using System;
using System.Linq.Expressions;

namespace ExpressionParser.Logic.Operators
{
    public class UnaryOperator : IOperatorInfo<Expression>
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Precedence => int.MaxValue;
        public Associativity Associativity => Associativity.Right;
        public int ParameterCount => 1;
        public Func<Expression, Expression> UnaryExpression { get; set; }

        Expression IOperatorInfo<Expression>.Reduce(Expression[] args)
        {
            return UnaryExpression(args[0]);
        }
    }
}
