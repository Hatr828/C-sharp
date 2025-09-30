using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace С_.Back
{
    public static class Operations
    {
        public static object? Add(object? a, object? b)  => (a, b) switch
        {
            (string s1, string s2) => s1 + s2,
            (string s, var v) => s + v,          
            (var v, string s) => $"{v}{s}",
            (long x, long y) => (object)(x + y),
            (int x, int y) => (object)(x + y),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };

        public static object? Minus(object? a, object? b) => (a, b) switch
        {
            (long x, long y) => (object)(x - y),
            (int x, int y) => (object)(x - y),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };

        public static object? Multiplay(object? a, object? b) => (a, b) switch
        {
            (long x, long y) => (object)(x * y),
            (int x, int y) => (object)(x * y),         

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };

        public static object? Divide(object? a, object? b) => (a, b) switch
        {
            (long x, long y) => (object)(x / y),
            (int x, int y) => (object)(x / y),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };

        public static object? Greater(object? a, object? b) => (a, b) switch
        {
            (long x, long y) => (object)(x > y),
            (int x, int y) => (object)(x > y),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };

        public static object? GreaterOrEqual(object? a, object? b) => (a, b) switch
        {
            (long x, long y) => (object)(x >= y),
            (int x, int y) => (object)(x >= y),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };

        public static object? Less(object? a, object? b) => (a, b) switch
        {
            (long x, long y) => (object)(x < y),
            (int x, int y) => (object)(x < y),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };

        public static object? LessOrEqual(object? a, object? b) => (a, b) switch
        {
            (long x, long y) => (object)(x <= y),
            (int x, int y) => (object)(x <= y),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {b.GetType()}")
        };
    }
}
