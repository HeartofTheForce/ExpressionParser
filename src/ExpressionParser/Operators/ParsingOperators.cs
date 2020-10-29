using System;
using System.Linq;

namespace ExpressionParser.Operators
{
    public static class ParsingMap
    {
        private static readonly OperatorInfo[] s_operatorMap = new OperatorInfo[]
        {
            new OperatorInfo("+", 0, Associativity.Right, false, true),
            new OperatorInfo("-", 0, Associativity.Right, false, true),
            new OperatorInfo("~", 0, Associativity.Right, false, true),
            new OperatorInfo("max", 0, Associativity.Right, false, true),
            new OperatorInfo("min", 0, Associativity.Right, false, true),
            new OperatorInfo("sin", 0, Associativity.Right, false, true),
            new OperatorInfo("cos", 0, Associativity.Right, false, true),
            new OperatorInfo("tan", 0, Associativity.Right, false, true),
            new OperatorInfo("clamp", 1, Associativity.Left, true, false),
            new OperatorInfo("*", 2, Associativity.Left, true, true),
            new OperatorInfo("/", 2, Associativity.Left, true, true),
            new OperatorInfo("+", 3, Associativity.Left, true, true),
            new OperatorInfo("-", 3, Associativity.Left, true, true),
            new OperatorInfo("&", 4, Associativity.Left, true, true),
            new OperatorInfo("^", 5, Associativity.Left, true, true),
            new OperatorInfo("|", 6, Associativity.Left, true, true),
        };

        public static bool Exists(string keyword) => s_operatorMap.Any(x => x.Keyword == keyword);
        public static OperatorInfo GetPrefix(string keyword) => s_operatorMap.SingleOrDefault(x => x.Keyword == keyword && x.IsPrefix());
        public static OperatorInfo GetPostfix(string keyword) => s_operatorMap.SingleOrDefault(x => x.Keyword == keyword && x.IsPostfix());
        public static OperatorInfo GetInfix(string keyword) => s_operatorMap.SingleOrDefault(x => x.Keyword == keyword && x.IsInfix());
    }
}
