using DbgCensus.EventStream.Abstractions.EventHandling;
using System;
using System.Linq;

namespace DbgCensus.EventStream.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsCensusEventHandler(this Type type)
        {
            Type[] interfaces = type.GetInterfaces();
            return interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICensusEventHandler<>));
        }
    }
}
