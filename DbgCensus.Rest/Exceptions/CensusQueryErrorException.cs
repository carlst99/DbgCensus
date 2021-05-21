using DbgCensus.Core.Exceptions;

namespace DbgCensus.Rest.Exceptions
{
    /// <summary>
    /// Stores information about an error that occured during an internal Census query.
    /// </summary>
#pragma warning disable RCS1194 // Implement exception constructors.
    public class CensusQueryErrorException : CensusException
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
        /// <param name="message">The returned Census error message.</param>
        /// <param name="code">The returned Census error code.</param>
        public CensusQueryErrorException(string? message, int? code)
            : base(message)
        {
            Code = code;
        }
    }
}
