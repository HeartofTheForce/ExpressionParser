using System;

namespace ExpressionParser.Operators
{
    public class OperatorInfo
    {
        public string Keyword { get; }
        public uint? LeftPrecedence { get; }
        public uint? RightPrecedence { get; }

        private OperatorInfo(
            string keyword,
            uint? leftPrecedence,
            uint? rightPrecedence)
        {
            Keyword = keyword;
            LeftPrecedence = leftPrecedence;
            RightPrecedence = rightPrecedence;
        }

        public static OperatorInfo Prefix(string keyword)
        {
            return new OperatorInfo(keyword, null, 1);
        }

        public static OperatorInfo Postfix(string keyword)
        {
            return new OperatorInfo(keyword, 0, null);
        }

        public static OperatorInfo Infix(string keyword, uint leftPrecedence, uint rightPrecedence)
        {
            return new OperatorInfo(keyword, leftPrecedence + 1, rightPrecedence + 1);
        }

        public bool IsPrefix() => LeftPrecedence == null && RightPrecedence != null;
        public bool IsPostfix() => LeftPrecedence != null && RightPrecedence == null;
        public bool IsInfix() => LeftPrecedence != null && RightPrecedence != null;

        public static bool IsEvaluatedBefore(OperatorInfo left, OperatorInfo right)
        {
            if (left.RightPrecedence == null || right.LeftPrecedence == null)
                return left.RightPrecedence == null;

            return left.RightPrecedence <= right.LeftPrecedence;
        }
    }
}
