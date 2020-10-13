using System;
using System.Linq.Expressions;

namespace ExpressionParser.Logic.Operators
{
    public class FunctionOperator : IOperatorInfo<Expression>
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Precedence => int.MaxValue;
        public Associativity Associativity => Associativity.Right;
        public int ParameterCount { get; set; }
        public Func<Expression[], Expression> FunctionExpression { get; set; }

        Expression IOperatorInfo<Expression>.Reduce(Expression[] args)
        {
            return FunctionExpression(args);
        }
    }
}
