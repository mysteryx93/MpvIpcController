using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Parses a value from type TApi to TResult.
    /// </summary>
    /// <typeparam name="TResult">The type to parse to.</typeparam>
    /// <typeparam name="TApi">The type to parse from.</typeparam>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsed value of type TResult.</returns>
    public delegate TResult PropertyParser<TResult, TApi>(TApi value);

    /// <summary>
    /// Formats a value from type TResult to type TApi.
    /// </summary>
    /// <typeparam name="TResult">The type to format from.</typeparam>
    /// <typeparam name="TApi">The type to format to.</typeparam>
    /// <param name="value">The formatted value of type TApi.</param>
    /// <returns></returns>
    public delegate TApi PropertyFormatter<TResult, TApi>(TResult value);
}
