using System;
using System.Collections.Generic;
using System.Linq;
using static ExpressionParser.Parser.Rules;

namespace ExpressionParser.Parser
{
    public static class Infix
    {
        public static string Infix2Postfix(ReadOnlySpan<char> infix)
        {
            var postfix = new List<string>();

            var depthStack = new Stack<Stack<IOperatorInfo>>();
            depthStack.Push(new Stack<IOperatorInfo>());

            string previousExpression = null;
            for (int i = 0; i < infix.Length; i++)
            {
                string current = infix.Slice(i, 1).ToString();
                if (string.IsNullOrWhiteSpace(current))
                {
                    current = null;
                }
                else if (current == "(")
                {
                    depthStack.Push(new Stack<IOperatorInfo>());
                }
                else if (current == ")")
                {
                    if (depthStack.Count == 1)
                        throw new Exception($"( Expected: {i}");

                    while (depthStack.Peek().Count > 0)
                    {
                        postfix.Add(depthStack.Peek().Pop().Output);
                    }
                    depthStack.Pop();
                }
                else
                {
                    current = TryParseExpression(infix[i..]).ToString();
                    var operatorInfo = TryParseOperator(previousExpression, current);

                    if (operatorInfo != null)
                    {
                        if (depthStack.Count == 0)
                            throw new Exception($"( Expected: {i}");

                        while (depthStack.Peek().Count > 0 && Evaluate(operatorInfo, depthStack.Peek().Peek()))
                        {
                            postfix.Add(depthStack.Peek().Pop().Output);
                        }
                        depthStack.Peek().Push(operatorInfo);
                    }
                    else
                    {
                        if (previousExpression == ")")
                            throw new Exception($"Operator Expected: {i}");

                        postfix.Add(current);
                    }

                    i += current.Length - 1;
                }

                if (current != null)
                    previousExpression = current;
            }

            if (depthStack.Count > 1)
                throw new Exception($") Expected: {infix.Length - 1}");

            if (depthStack.Count == 0)
                throw new Exception($"( Expected: {infix.Length - 1}");

            while (depthStack.Peek().Count > 0)
            {
                postfix.Add(depthStack.Peek().Pop().Output);
            }

            return string.Join(' ', postfix);
        }

        private static bool Evaluate(IOperatorInfo current, IOperatorInfo previous)
        {
            if (current.Precedence == previous.Precedence)
            {
                if (current.Associativity == previous.Associativity)
                {
                    switch (current.Associativity)
                    {
                        case Associativity.Left: return true;
                        case Associativity.Right: return false;
                    }
                }
                else
                {
                    throw new Exception("Operators with same Precedence must have same Associativity");
                }
            }

            return current.Precedence < previous.Precedence;
        }

        private static ReadOnlySpan<char> TryParseExpression(ReadOnlySpan<char> input)
        {
            bool isAlphanumeric = char.IsLetterOrDigit(input[0]);

            int i;
            for (i = 0; i < input.Length; i++)
            {
                char current = input[i];

                if (isAlphanumeric && !char.IsLetterOrDigit(current))
                    break;
                if (!isAlphanumeric && char.IsLetterOrDigit(current))
                    break;
                if (char.IsWhiteSpace(current) || current == '(' || current == ')')
                    break;
            }

            return input.Slice(0, i);
        }

        private static IOperatorInfo TryParseOperator(string previousExpression, string currentOperator)
        {
            bool isFunction =
                previousExpression == null ||
                previousExpression == "(" ||
                BinaryOperatorMap.Any(x => x.Input == previousExpression) ||
                FunctionOperatorMap.Any(x => x.Input == previousExpression);

            var functionMeta = FunctionOperatorMap.FirstOrDefault(x => x.Input == currentOperator);
            var binaryMeta = BinaryOperatorMap.FirstOrDefault(x => x.Input == currentOperator);

            if (isFunction)
            {
                if (functionMeta != null)
                    return functionMeta;
                else if (binaryMeta != null)
                    throw new Exception($"Incorrectly used Binary Operator: {currentOperator}");
            }
            else
            {
                if (binaryMeta != null)
                    return binaryMeta;
                else if (functionMeta != null)
                    throw new Exception($"Incorrectly used Function Operator: {currentOperator}");
            }

            return null;
        }
    }
}
