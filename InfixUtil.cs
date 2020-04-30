using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public static class InfixUtil
    {
        public static string Infix2Postfix(ReadOnlySpan<char> infix)
        {
            var expression = new List<string>();

            var depthStack = new Stack<Stack<OperatorMeta>>();
            depthStack.Push(new Stack<OperatorMeta>());

            string lastExpression = null;
            for (int i = 0; i < infix.Length; i++)
            {
                string operand = ReadUntilNonAlphaNumeric(infix.Slice(i)).ToString();
                if (operand.Length > 0)
                {
                    expression.Add(operand);

                    i += operand.Length - 1;
                    lastExpression = operand;
                    continue;
                }

                string current = infix[i].ToString();
                if (!string.IsNullOrWhiteSpace(current))
                {
                    if (current == "(")
                    {
                        depthStack.Push(new Stack<OperatorMeta>());
                    }
                    else if (current == ")")
                    {
                        while (depthStack.Peek().Count > 0)
                        {
                            expression.Add(depthStack.Peek().Pop().Output);
                        }
                        depthStack.Pop();
                    }
                    else
                    {
                        var operatorMeta = ParseOperator(lastExpression, current);
                        while (depthStack.Peek().Count > 0 && Evaluate(operatorMeta, depthStack.Peek().Peek()))
                        {
                            expression.Add(depthStack.Peek().Pop().Output);
                        }
                        depthStack.Peek().Push(operatorMeta);
                    }

                    lastExpression = current;
                }
            }

            while (depthStack.Peek().Count > 0)
            {
                expression.Add(depthStack.Peek().Pop().Output);
            }

            return string.Join(' ', expression);
        }

        private static bool Evaluate(OperatorMeta current, OperatorMeta previous)
        {
            if (current.Precedence == previous.Precedence)
            {
                if (current.Associativity == previous.Associativity)
                {
                    switch (current.Associativity)
                    {
                        case  Associativity.Left: return true;
                        case  Associativity.Right: return false;
                    }
                }
                else
                {
                    throw new Exception("Operators with same Precedence must have same ");
                }
            }

            return current.Precedence < previous.Precedence;
        }

        private static OperatorMeta ParseOperator(string previousExpression, string currentOperator)
        {
            bool isUnary = previousExpression == null || previousExpression == "(" || s_binaryOperatorMap.ContainsKey(previousExpression) || s_unaryOperatorMap.ContainsKey(previousExpression);
            if (isUnary)
            {
                if (s_unaryOperatorMap.TryGetValue(currentOperator, out var meta))
                {
                    return meta;
                }

                throw new Exception("Invalid Unary Operator");
            }
            else
            {
                if (s_binaryOperatorMap.TryGetValue(currentOperator, out var meta))
                {
                    return meta;
                }

                throw new Exception("Invalid Binary Operator");
            }
        }

        public static string Infix2Prefix(string infix)
        {
            string reverse = ReverseExpression(infix);
            string postfix = Infix2Postfix(reverse);
            string prefix = ReverseExpression(postfix);
            return prefix;
        }

        static string ReverseExpression(string input)
        {
            char[] reverse = new char[input.Length];
            for (int i = 0; i < reverse.Length; i++)
            {
                if (input[i] == '(')
                    reverse[reverse.Length - 1 - i] = ')';
                else if (input[i] == ')')
                    reverse[reverse.Length - 1 - i] = '(';
                else
                    reverse[reverse.Length - 1 - i] = input[i];
            }
            return new string(reverse);
        }

        private static ReadOnlySpan<char> ReadUntilNonAlphaNumeric(ReadOnlySpan<char> input)
        {
            int i = 0;
            while (i < input.Length)
            {
                char current = input[i];

                if (!char.IsLetterOrDigit(current))
                {
                    break;
                }
                i++;
            }

            return input.Slice(0, i);
        }

        public struct OperatorMeta
        {
            public int Precedence { get; set; }
            public Associativity Associativity { get; set; }
            public string Output { get; set; }
        }

        public enum Associativity
        {
            Left,
            Right
        }

        static readonly Dictionary<string, OperatorMeta> s_binaryOperatorMap = new Dictionary<string, OperatorMeta>()
        {
            { "+", new OperatorMeta(){ Precedence = 1, Associativity = Associativity.Left, Output = "+" }},
            { "-", new OperatorMeta(){ Precedence = 1, Associativity = Associativity.Left, Output = "-" }},
            { "*", new OperatorMeta(){ Precedence = 2, Associativity = Associativity.Left, Output = "*" }},
            { "/", new OperatorMeta(){ Precedence = 2, Associativity = Associativity.Left, Output = "/" }},
            { "|", new OperatorMeta(){ Precedence = 3, Associativity = Associativity.Right, Output = "|" }},
            { "^", new OperatorMeta(){ Precedence = 4, Associativity = Associativity.Right, Output = "^" }},
            { "&", new OperatorMeta(){ Precedence = 4, Associativity = Associativity.Right, Output = "&" }},
        };

        static readonly Dictionary<string, OperatorMeta> s_unaryOperatorMap = new Dictionary<string, OperatorMeta>()
        {
            { "+", new OperatorMeta(){ Precedence = int.MaxValue, Associativity= Associativity.Right, Output = "u+"}},
            { "-", new OperatorMeta(){ Precedence = int.MaxValue, Associativity= Associativity.Right, Output = "u-"}},
            { "~", new OperatorMeta(){ Precedence = int.MaxValue, Associativity= Associativity.Right, Output = "~"}},
        };
    }
}
