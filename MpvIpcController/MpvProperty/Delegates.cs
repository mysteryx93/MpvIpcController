using System;
using System.Diagnostics.CodeAnalysis;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Parses a value from type TApi to TResult.
    /// </summary>
    /// <typeparam name="TResult">The type to parse to.</typeparam>
    /// <typeparam name="TApi">The type to parse from.</typeparam>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsed value of type TResult.</returns>
    [return: MaybeNull]
    public delegate T CustomParser<T>(MpvResponse? value);

    /// <summary>
    /// Formats a value from type TResult to type TApi.
    /// </summary>
    /// <typeparam name="TResult">The type to format from.</typeparam>
    /// <typeparam name="TApi">The type to format to.</typeparam>
    /// <param name="value">The formatted value of type TApi.</param>
    /// <returns></returns>
    public delegate object? CustomFormatter<T>([AllowNull] T value);
}
