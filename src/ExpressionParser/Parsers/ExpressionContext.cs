using System;
using System.Collections.Generic;
using ExpressionParser.Operators;

namespace ExpressionParser.Parsers
{
    public class ExpressionContext
    {
        private readonly Queue<Node> _operands;
        private readonly Stack<OperatorNode> _operators;

        public bool HaveOperand()
        {
            return _operands.Count > 0 || (_operators.Count > 0 && !_operators.Peek().OperatorInfo.HasRightArguments);
        }

        public ExpressionContext()
        {
            _operands = new Queue<Node>();
            _operators = new Stack<OperatorNode>();
        }

        public void PushOperand(Node operand)
        {
            _operands.Enqueue(operand);
        }

        public void PushOperator(OperatorInfo operatorInfo)
        {
            while (_operators.TryPeek(out var left) && OperatorInfo.IsEvaluatedBefore(left.OperatorInfo, operatorInfo))
            {
                PopOperator();
            }

            bool hasLeftArguments = _operands.Count > 0;
            if (hasLeftArguments != operatorInfo.HasLeftArguments)
                throw new Exception("Unexpected left arguments");

            var operatorNode = new OperatorNode(operatorInfo);
            while (_operands.Count > 0)
            {
                operatorNode.Children.Add(_operands.Dequeue());
            }

            _operators.Push(operatorNode);
        }

        public Node Reduce()
        {
            while (_operators.Count > 0)
            {
                PopOperator();
            }

            if (_operands.Count != 1)
                throw new ExpressionReductionException(_operands.Count);

            return _operands.Dequeue();
        }

        private void PopOperator()
        {
            var operatorNode = _operators.Pop();

            bool hasRightArguments = _operands.Count > 0;
            if (hasRightArguments != operatorNode.OperatorInfo.HasRightArguments)
                throw new Exception("Unexpected right arguments");

            while (_operands.Count > 0)
            {
                operatorNode.Children.Add(_operands.Dequeue());
            }

            _operands.Enqueue(operatorNode);
        }
    }
}
