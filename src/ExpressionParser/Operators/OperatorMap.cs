using System.Linq;

namespace ExpressionParser.Operators
{
    public static class OperatorMap
    {
        public static bool Exists(this OperatorInfo[] operatorMap, string keyword)
        {
            return operatorMap.Any(x => x.Keyword == keyword);
        }

        public static OperatorInfo GetPrefix(this OperatorInfo[] operatorMap, string keyword)
        {
            return operatorMap.SingleOrDefault(x => x.Keyword == keyword && x.IsPrefix());
        }

        public static OperatorInfo GetPostfix(this OperatorInfo[] operatorMap, string keyword)
        {
            return operatorMap.SingleOrDefault(x => x.Keyword == keyword && x.IsPostfix());
        }

        public static OperatorInfo GetInfix(this OperatorInfo[] operatorMap, string keyword)
        {
            return operatorMap.SingleOrDefault(x => x.Keyword == keyword && x.IsInfix());
        }
    }
}
