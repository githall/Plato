using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PlatoCore.Abstractions.Extensions
{

    public static class TypeExtensions
    {

        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool IsAnonymousType(this Type t)
        {
            // This code could potentially be executed within a hot code path
            // For this reason we intentionally avoid linq here
            // Anonymous types are marked with the CompilerGeneratedAttribute attribute by the compiler
            // If we find this attribute at run-time returned true
            var attrs = t?.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true);
            return attrs != null && attrs.Length > 0;
        }

    }

}
