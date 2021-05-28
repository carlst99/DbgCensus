using System.Linq;

namespace DbgCensus.Core.Utils
{
    public static class StringUtils
    {
        public static string JoinWithoutNullOrEmptyValues(char separator, params string[] value) => string.Join(separator, value.Where(str => !string.IsNullOrEmpty(str)));
    }
}
