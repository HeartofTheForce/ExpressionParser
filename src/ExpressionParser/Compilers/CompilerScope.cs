using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionParser.Parsers;

namespace ExpressionParser.Compilers
{
    public class CompilerFunction<T>
    {
        public string Name { get; }
        public int ArgumentCount { get; }
        public Func<T[], T> Reduce { get; }

        public CompilerFunction(
            string name,
            int argumentCount,
            Func<T[], T> reduce)
        {
            Name = name;
            ArgumentCount = argumentCount;
            Reduce = reduce;
        }
    }

    public class CompilerScope<T>
    {
        public Func<LeafNode, T> CompileLeafNode { get; }
        public IEnumerable<CompilerFunction<T>> Functions { get; }

        public CompilerScope(
            Func<LeafNode, T> compileLeafNode,
            IEnumerable<CompilerFunction<T>> functions)
        {
            CompileLeafNode = compileLeafNode;
            Functions = functions;
        }

        public T CompileNode(Node node)
        {
            if (node is LeafNode leafNode)
                return CompileLeafNode(leafNode);
            else if (node is OperatorNode operatorNode)
            {
                var functions = Functions.Where(x => x.Name == operatorNode.OperatorInfo.Keyword);
                if (!functions.Any())
                    throw new MissingFunctionException(operatorNode);

                var function = functions.SingleOrDefault(x => x.ArgumentCount == operatorNode.Children.Count);
                if (function == null)
                    throw new OverloadMismatchException(operatorNode);

                var arguments = new T[operatorNode.Children.Count];
                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = CompileNode(operatorNode.Children[i]);
                }

                return function.Reduce(arguments);
            }
            else
                throw new Exception($"Cannot compile node '{node.GetType()}'");
        }
    }
}
