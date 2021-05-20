using System;

namespace DbgCensus.Rest.Exceptions
{
    /// <summary>
    /// Indicates that a Census service is unavailable.
    /// </summary>
#pragma warning disable RCS1194 // Implement exception constructors.
    public class CensusServiceUnavailableException : Exception
#pragma warning restore RCS1194 // Implement exception constructors.
    {
        /// <summary>
        /// Indicates that a Census service is unavailable.
        /// </summary>
        public CensusServiceUnavailableException()
        {
        }
    }
}
