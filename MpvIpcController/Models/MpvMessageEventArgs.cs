namespace HanumanInstitute.MpvIpcController;

/// <summary>
/// Contains information about an event message received from MPV.
/// </summary>
public class MpvMessageEventArgs : EventArgs
{
    public MpvMessageEventArgs(MpvEvent obj)
    {
        obj.CheckNotNull(nameof(obj));

        EventName = obj.Event;
        Data = obj.Data;
    }

    public string EventName { get; set; } = string.Empty;
    public IDictionary<string, string?> Data { get; } = new Dictionary<string, string?>();
}