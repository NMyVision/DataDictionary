using System;
using System.Linq;
using System.Reflection;

namespace NMyVision.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determine if the Type is an anonymous object.
        /// </summary>
        /// <param name="type">Type being tested.</param>
        public static Boolean IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetTypeInfo().GetCustomAttributes(
                typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                false).Any();
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");

            return hasCompilerGeneratedAttribute && nameContainsAnonymousType;
        }

        /// <summary>
        /// Determine if the type is a nullable type.
        /// </summary>
        /// <param name="type">Type being tested.</param>
        public static bool IsNullableType(this Type type)
        {
            return (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }

}
