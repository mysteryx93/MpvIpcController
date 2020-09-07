using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public interface IMpvApi : IMpvController
    {
        /// <summary>
        /// Returns the name of the client as string. This is the string ipc-N with N being an integer number.
        /// </summary>
        Task<string> GetClientName();
        /// <summary>
        /// Returns the current mpv internal time in microseconds as a number. This is basically the system time, with an arbitrary offset.
        /// </summary>
        Task<int> GetClientTime();
        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        Task<T> GetProperty<T>(string propertyName) where T : struct;
        /// <summary>
        /// Returns the value of the given property. The resulting data will always be a string.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        Task<string> GetPropertyString(string propertyName);
        /// <summary>
        /// Sets the given property to the given value.
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        Task SetProperty(string propertyName, object value);
        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
        /// </summary>
        /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        Task ObserveProperty(int observeId, string propertyName);
        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated. The resulting data will always be a string.
        /// </summary>
        /// <param name="observeId">An ID for the observer.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        Task ObservePropertyString(int observeId, string propertyName);
        /// <summary>
        /// Undo ObserveProperty or ObservePropertyString. This requires the numeric id passed to the observed command as argument.
        /// </summary>
        /// <param name="observeId">The ID of the observer.</param>
        Task UnobserveProperty(int observeId);
        /// <summary>
        /// Enable output of mpv log messages. They will be received as events.
        /// Log message output is meant for humans only (mostly for debugging). Attempting to retrieve information by parsing these messages will just lead to breakages with future mpv releases.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        Task RequestLogMessages(LogLevel logLevel);
        /// <summary>
        /// Enables the named event. If the string 'all' is used instead of an event name, all events are enabled.
        /// By default, most events are enabled, and there is not much use for this command.
        /// </summary>
        /// <param name="eventName">The name of the event to enable.</param>
        Task EnableEvent(string eventName);
        /// <summary>
        /// Disables the named event. If the string 'all' is used instead of an event name, all events are disabled.
        /// </summary>
        /// <param name="eventName">The name of the event to enable.</param>
        Task DisableEvent(string eventName);
        /// <summary>
        /// Returns the client API version the C API of the remote mpv instance provides.
        /// </summary>
        Task<int> GetVersion();
    }
}
