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

            var unaryStack = new Stack<Stack<string>>();
            unaryStack.Push(new Stack<string>());
            var binaryStack = new Stack<string>();

            string lastExpression = null;
            for (int i = 0; i < infix.Length; i++)
            {
                string operand = ReadUntilNonAlphaNumeric(infix.Slice(i)).ToString();
                if (operand.Length > 0)
                {
                    expression.Add(operand);

                    while (unaryStack.Peek().Count > 0)
                    {
                        expression.Add(unaryStack.Peek().Pop());
                    }
                    i += operand.Length - 1;
                    lastExpression = operand;
                    continue;
                }

                string current = infix[i].ToString();
                if (!int.TryParse(current, out _) && !string.IsNullOrWhiteSpace(current))
                {
                    if (current == "(")
                    {
                        unaryStack.Push(new Stack<string>());
                        binaryStack.Push(current);
                    }
                    else if (current == ")")
                    {
                        while (binaryStack.Peek() != "(")
                        {
                            expression.Add(binaryStack.Pop());
                        }
                        binaryStack.Pop();

                        unaryStack.Pop();
                        while (unaryStack.Peek().Count > 0)
                        {
                            expression.Add(unaryStack.Peek().Pop());
                        }
                    }
                    else
                    {
                        current = ParseOperator(lastExpression, current);
                        if (UnaryOperatorSet.Contains(current))
                        {
                            unaryStack.Peek().Push(current);
                        }
                        else
                        {
                            while (binaryStack.Count > 0 && Precedence(current) <= Precedence(binaryStack.Peek()))
                            {
                                expression.Add(binaryStack.Pop());
                            }
                            binaryStack.Push(current);
                        }
                    }

                    lastExpression = current;
                }
            }

            while (binaryStack.Count > 0)
            {
                expression.Add(binaryStack.Pop());
            }

            return string.Join(' ', expression);
        }

        private static string ParseOperator(string previousExpression, string currentOperator)
        {
            bool isUnary = previousExpression == null || previousExpression == "(" || BinaryOperatorSet.Contains(previousExpression) || UnaryOperatorSet.Contains(previousExpression);
            if (isUnary)
            {
                if (UnaryMap.TryGetValue(currentOperator, out string unaryOperator))
                    currentOperator = unaryOperator;

                if (UnaryOperatorSet.Contains(currentOperator))
                    return currentOperator;

                throw new Exception("Invalid Unary Operator");
            }
            else
            {
                if (BinaryOperatorSet.Contains(currentOperator))
                    return currentOperator;

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

        static int Precedence(string c)
        {
            if (PrecedenceMap.TryGetValue(c, out int precedence))
            {
                return precedence;
            }

            throw new Exception("Invalid Operator");
        }

        static Dictionary<string, int> PrecedenceMap = new Dictionary<string, int>()
        {
            { "(", 0 },
            { ")", 0 },
            { "+", 1 },
            { "-", 1 },
            { "*", 2 },
            { "/", 2 },
            { "^", 3 },
        };

        static HashSet<string> BinaryOperatorSet = new HashSet<string>()
        {
            "+",
            "-",
            "*",
            "/",
            "^",
        };

        static HashSet<string> UnaryOperatorSet = new HashSet<string>()
        {
            "u+",
            "u-",
            "~",
        };

        static Dictionary<string, string> UnaryMap = new Dictionary<string, string>()
        {
            { "+", "u+" },
            { "-", "u-" },
        };
    }
}
