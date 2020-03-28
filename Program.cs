using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string infix = "a+b*(c^d-e)^(f+g*h)-i";
            string postfix = Infix2Postfix(infix);
            string prefix = Infix2Prefix(infix);
            Console.WriteLine($"Postfix: {postfix}");
            Console.WriteLine($"Prefix : {prefix}");

            //Expected Postfix abcd^e-fgh*+^*+i-
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

        static string Infix2Prefix(string infix)
        {
            string reverse = ReverseExpression(infix);
            string postfix = Infix2Postfix(reverse);
            string prefix = ReverseExpression(postfix);
            return prefix;
        }

        static string Infix2Postfix(ReadOnlySpan<char> infix)
        {
            var sb = new StringBuilder();
            var operatorStack = new Stack<char>();

            for (int i = 0; i < infix.Length; i++)
            {
                var operand = ReadUntilNonAlphaNumeric(infix.Slice(i));
                if (operand.Length > 0)
                {
                    sb.Append(' ');
                    sb.Append(operand);
                    i += operand.Length - 1;
                    continue;
                }

                char current = infix[i];
                if (!char.IsLetterOrDigit(current) && current != ' ')
                {
                    if (current == '(')
                    {
                        operatorStack.Push(current);
                    }
                    else if (current == ')')
                    {
                        while (operatorStack.Peek() != '(')
                        {
                            sb.Append(' ');
                            sb.Append(operatorStack.Pop());
                        }
                        operatorStack.Pop();
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && Precedence(current) <= Precedence(operatorStack.Peek()))
                        {
                            sb.Append(' ');
                            sb.Append(operatorStack.Pop());
                        }
                        operatorStack.Push(current);
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                sb.Append(' ');
                sb.Append(operatorStack.Pop());
            }

            return sb.ToString();
        }

        private static ReadOnlySpan<char> ReadUntilNonAlphaNumeric(ReadOnlySpan<char> input)
        {
            int i = 0;
            while (i < input.Length)
            {
                char current = input[i];

                if (!char.IsLetterOrDigit(current))
                    break;

                i++;
            }

            return input.Slice(0, i);
        }

        static int Precedence(char c)
        {
            if (PrecedenceMap.TryGetValue(c, out int precedence))
            {
                return precedence;
            }

            throw new Exception("Invalid Operator");
        }

        static Dictionary<char, int> PrecedenceMap = new Dictionary<char, int>()
        {
            { '(', 0 },
            { ')', 0 },
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 },
            { '^', 3 },
        };
    }
}
