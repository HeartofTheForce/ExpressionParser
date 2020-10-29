using System;
using ExpressionParser.Parsers;

namespace ExpressionParser.Compilers
{
    public class MissingFunctionException : Exception
    {
        public OperatorNode OperatorNode { get; set; }

        public MissingFunctionException(OperatorNode operatorNode)
        : base($"Missing function '{operatorNode.OperatorInfo.Keyword}'")
        {
            OperatorNode = operatorNode;
        }
    }

    public class OverloadMismatchException : Exception
    {
        public OperatorNode OperatorNode { get; set; }

        public OverloadMismatchException(OperatorNode operatorNode)
        : base($"No overload for '{operatorNode.OperatorInfo.Keyword}' takes {operatorNode.Children.Count} arguments")
        {
            OperatorNode = operatorNode;
        }
    }
}
