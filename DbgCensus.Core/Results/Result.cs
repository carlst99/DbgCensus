using System;

namespace DbgCensus.Core.Results
{
    internal class Result : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public Exception? Error { get; }

        protected Result(bool isSuccess, Exception? ex)
        {
            IsSuccess = isSuccess;
            Error = ex;
        }

        /// <summary>
        /// Returns a failed result
        /// </summary>
        /// <param name="error">An optional error payload</param>
        public static Result FromError(Exception? error) => new(false, error);

        /// <summary>
        /// Returns a failed result
        /// </summary>
        /// <param name="failedResult">An existing failed result with an error</param>
        public static Result FromError(IResult failedResult) => new(false, failedResult.Error);

        /// <summary>
        /// Returns a successful result
        /// </summary>
        public static Result FromSuccess() => new(true, null);
    }

    internal class Result<T> : IResult<T>
    {
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public Exception? Error { get; }

        /// <inheritdoc/>
        public T? Value { get; }

        protected Result(bool isSuccess, Exception? error, T? value)
        {
            IsSuccess = isSuccess;
            Error = error;
            Value = value;
        }

        /// <summary>
        /// Returns a failed result
        /// </summary>
        /// <param name="error">An optional error payload</param>
        public static Result<T> FromError(Exception? error) => new(false, error, default);

        /// <summary>
        /// Returns a failed result
        /// </summary>
        /// <param name="failedResult">An existing failed result with an error</param>
        public static Result<T> FromError(IResult failedResult) => new(false, failedResult.Error, default);

        /// <summary>
        /// Returns a successful result
        /// </summary>
        /// <param name="value">The value of the result</param>
        public static Result<T> FromSuccess(T value) => new(true, null, value);

        /// <summary>
        /// Returns a successful result
        /// </summary>
        /// <param name="successfulResult">An existing successful result with a value</param>
        /// <returns></returns>
        public static Result<T> FromSuccess(Result<T> successfulResult) => new(true, null, successfulResult.Value);
    }
}
