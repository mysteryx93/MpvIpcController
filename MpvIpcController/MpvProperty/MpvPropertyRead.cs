using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read-only MPV property. T must be a nullable type.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyRead<T> : MpvProperty<T>
    {
        private readonly CustomParser<T>? _parser;

        public MpvPropertyRead(MpvApi api, string name, CustomParser<T>? parser = null) : base(api, name)
        {
            _parser = parser;
        }

        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        public async Task<MpvResponse<T>?> GetAsync(ApiOptions? options = null)
        {
            var result = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
            if (_parser != null)
            {
                return result.Copy(_parser(result));
            }
            return result.Parse<T>();
        }
    }
}
