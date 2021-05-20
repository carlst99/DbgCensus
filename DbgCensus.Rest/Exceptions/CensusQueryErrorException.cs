using System;

namespace DbgCensus.Rest.Exceptions
{
    /// <summary>
    /// Stores information about an error that occured during an internal Census query.
    /// </summary>
#pragma warning disable RCS1194 // Implement exception constructors.
    public class CensusQueryErrorException : Exception
#pragma warning restore RCS1194 // Implement exception constructors.
    {
        /// <summary>
        /// The Census error code.
        /// </summary>
        public int? Code { get; }

        /// <summary>
        /// Stores information about an error that occured during an internal Census query.
        /// </summary>
        /// <param name="message">The returned Census error messasge.</param>
        public CensusQueryErrorException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Stores information about an error that occured during an internal Census query.
        /// </summary>
        /// <param name="code">The returned Census error code.</param>
        /// <param name="message">The returned Census error message.</param>
        public CensusQueryErrorException(string? message, int? code)
            : base(message)
        {
            Code = code;
        }
    }
}
