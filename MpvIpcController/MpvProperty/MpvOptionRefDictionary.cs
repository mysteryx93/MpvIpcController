using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// This variant of MpvOptionDictionary doesn't use ChangeList and gets and sets values as a direct property. Values are not escaped.
    /// </summary>
    public class MpvOptionRefDictionary : MpvOptionRef<IDictionary<string, string>>
    {
        private readonly char _separator;

        public MpvOptionRefDictionary(MpvApi api, string name, bool isPath = false) : base(api, name)
        {
            _separator = isPath ? System.IO.Path.PathSeparator : ',';
        }

        private static string FormatKeyValue(string key, string value) => key + "=" + value;

        private string FormatKeyValueList(IDictionary<string, string> values) => string.Join(_separator.ToString(), values.Select(x => FormatKeyValue(x.Key, x.Value)));

        /// <summary>
        /// Sets a dictionary of key/value pairs.
        /// </summary>
        public override Task SetAsync(IDictionary<string, string> values, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, FormatKeyValueList(values), options);
    }
}
