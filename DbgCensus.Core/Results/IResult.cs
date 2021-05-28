using System;

namespace DbgCensus.Core.Results
{
    internal interface IResult
    {
        /// <summary>
        /// Gets a value indicating whether this is a successful result
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// An exception that may have occured while processing the result
        /// </summary>
        Exception? Error { get; }
    }

    internal interface IResult<T> : IResult
    {
        /// <summary>
        /// A value that may have been obtained while processing the result
        /// </summary>
        T? Value { get; }
    }
}
