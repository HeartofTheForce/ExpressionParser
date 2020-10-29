using System.Collections.Generic;
using System.Linq;
using ExpressionParser.Operators;

namespace ExpressionParser.Parsers
{
    public class OperatorNode : Node
    {
        public OperatorInfo OperatorInfo { get; }
        public List<Node> Children { get; }

        public OperatorNode(OperatorInfo operatorInfo)
        {
            OperatorInfo = operatorInfo;
            Children = new List<Node>();
        }

        public override string ToString()
        {
            string children = string.Join(" ", Children.Select(x => x.ToString()));
            return $"({OperatorInfo.Keyword} {children})";
        }
    }

    public class LeafNode : Node
    {
        public string Value { get; }

        public LeafNode(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public abstract class Node
    {
    }
}
