using System;

namespace ExpressionParser.Compilers
{
    public static class TypeUtility
    {
        public static bool IsFloat(Type type) => type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        public static bool IsInteger(Type type) =>
                type == typeof(sbyte) || type == typeof(byte) ||
                type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
    }
}
