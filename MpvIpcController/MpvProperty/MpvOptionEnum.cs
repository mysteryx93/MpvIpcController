﻿namespace HanumanInstitute.MpvIpcController;

public class MpvOptionEnum<T> : MpvPropertyWrite<T>
    where T : struct, Enum
{
    public MpvOptionEnum(MpvApi api, string name) : base(api, name)
    {
    }

    /// <summary>
    /// Parse value as specified type.
    /// </summary>
    /// <param name="value">The raw value to parse.</param>
    /// <returns>The typed parsed value.</returns>
    protected override T? ParseValue(string? value)
    {
        if (value == null)
        {
            return null;
        }

        if (string.Compare((string)value, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
        {
            value = "yes";
        }
        if (string.Compare((string)value, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
        {
            value = "no";
        }

        // Remove parenthesis.
        if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
        {
            value = value.Substring(1, (int)(value.Length - 2));
        }
        return FlagExtensions.ParseMpvFlag<T>((string)value);
    }

    /// <summary>
    /// Formats specified value to send into a MPV request.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted value.</returns>
    protected override string? FormatValue(T? value) => value?.FormatMpvFlag() ?? null;

    public override Task AddAsync(T value, ApiOptions? options = null) => throw new NotImplementedException();

    public override Task MultiplyAsync(double value, ApiOptions? options = null) => throw new NotImplementedException();
}
