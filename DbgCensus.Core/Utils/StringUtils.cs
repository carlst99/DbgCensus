using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Core.Utils;

public static class StringUtils
{
    public static string JoinWithoutNullOrEmptyValues(char separator, params string?[] values)
        => JoinWithoutNullOrEmptyValues(separator, (IEnumerable<string?>)values);

    public static string JoinWithoutNullOrEmptyValues(char separator, IEnumerable<string?> values)
        => string.Join
        (
            separator,
            values.Where(str => !string.IsNullOrEmpty(str))
        );

    /// <summary>
    /// Converts an object to a string, checking that ToString() has been properly implemented.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <param name="item">The object to convert.</param>
    /// <returns>The string representation of the object.</returns>
    public static string SafeToString<TObject>(TObject? item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        string? typeName = typeof(TObject).FullName;
        string? value = item.ToString();

        if (string.IsNullOrEmpty(value) || value.Equals(typeName))
        {
            throw new ArgumentException
            (
                $"The type {typeName} must have properly implemented ToString()",
                nameof(item)
            );
        }

        return value;
    }
}
