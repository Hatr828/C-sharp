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
            (long i1, long i2) => i1 + i2, 

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {a.GetType()}")
        };

        public static object? Multiplay(object? a, object? b) => (a, b) switch
        {
            (long i1, long i2) => i1 * i2,

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {a.GetType()}")
        };

        public static object? Divide(object? a, object? b) => (a, b) switch
        {
            (long i1, long i2) => (long)(i1 / i2),

            _ => throw new InvalidOperationException($"Unsupported types: {a.GetType()} and {a.GetType()}")
        };
    }
}
