using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Core.Utils
{
    public static class StringUtils
    {
        public static string JoinWithoutNullOrEmptyValues(char separator, params string[] values) => string.Join(separator, values.Where(str => !string.IsNullOrEmpty(str)));
        public static string JoinWithoutNullOrEmptyValues(char separator, IEnumerable<string?> values) => string.Join(separator, values.Where(str => !string.IsNullOrEmpty(str)));

        /// <summary>
        /// Converts an object to a string, checking that ToString() has been properly implemented.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="element">The object to convert.</param>
        /// <returns>The string representation of the object.</returns>
        public static string SafeToString<T>(T element) where T : notnull
        {
            string? typeName = typeof(T).FullName;
            string? value = element.ToString();

            if (string.IsNullOrEmpty(value) || value == typeName)
                throw new ArgumentException("The type " + typeName + " must have properly implemented ToString()", nameof(element));

            return value;
        }
    }
}
