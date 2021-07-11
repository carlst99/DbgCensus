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
        /// <param name="element">The object to convert.</param>
        /// <returns>The string representation of the object.</returns>
        public static string SafeToString(object? element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            string? typeName = element.GetType().FullName;
            string? value = element.ToString();

            if (string.IsNullOrEmpty(value) || value.Equals(typeName))
                throw new ArgumentException("The type " + typeName + " must have properly implemented ToString()", nameof(element));

            return value;
        }
    }
}
