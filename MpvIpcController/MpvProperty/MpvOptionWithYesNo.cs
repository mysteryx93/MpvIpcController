namespace HanumanInstitute.MpvIpcController;

public class MpvOptionWithYesNo<T> : MpvOptionWithNo<T>
    where T : struct
{
    public MpvOptionWithYesNo(MpvApi api, string name) :
        base(api, name)
    {
    }

    /// <summary>
    /// Sets the option to 'yes'.
    /// </summary>
    public Task SetYesAsync(ApiOptions? options = null) => SetValueAsync("yes", options);

    /// <summary>
    /// Gets whether the option is 'yes'.
    /// </summary>
    public Task<bool> GetYesAsync(ApiOptions? options = null) => GetValueAsync(new[] { "yes", "true" }, options);
}