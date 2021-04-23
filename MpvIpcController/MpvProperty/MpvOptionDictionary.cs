using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a comma-delimited option list of key-value pairs. ex: Val1=a,Val2=b
    /// </summary>
    public class MpvOptionDictionary : MpvOptionRef<IDictionary<string, string>>
    {
        // Note: API doesn't support escaping, so there's no way of interpreting values containing a separator.
        // As a reliable work-around, all APIs interpreting separators are discarted. The implemented methods work reliably with any values.
        // '=' is also prohibited in keys to avoid conflicts.

        public MpvOptionDictionary(MpvApi api, string name) : base(api, name)
        {
            //private readonly char _separator;
            // , bool isPath = false
            //_separator = isPath ? System.IO.Path.PathSeparator : ',';
        }

        //protected override IDictionary<string, string>? ParseValue(string? value)
        //{
        //    var listSplit = value?.Split(_separator) ?? Array.Empty<string>();
        //    var result = new Dictionary<string, string>(listSplit.Length);
        //    foreach (var item in listSplit)
        //    {
        //        var pair = ParseKeyValue(item);
        //        result.Add(pair.Key, pair.Value);
        //    }
        //    return result;
        //}

        //protected override string? FormatValue(IDictionary<string, string>? values)
        //{
        //    return string.Join(_separator.ToString(), values.Select(x => FormatKeyValue(x.Key, x.Value)));
        //}

        //private static KeyValuePair<string, string> ParseKeyValue(string data)
        //{
        //    var dataSplit = data.Split('=');
        //    if (!dataSplit.Any())
        //    {
        //        throw new FormatException($"Key-value pair '{data}' is not in a valid format.");
        //    }
        //    var key = dataSplit[0];
        //    var value = dataSplit.Length > 1 ? dataSplit[1] : string.Empty;
        //    return new(key, value);
        //}

        //private static string FormatKeyValue(string key, string value) => // "%{0}%{1}=%{2}%{3}".FormatInvariant(key?.Length ?? 0, key, value?.Length ?? 0, value);
        private static string FormatKeyValue(string key, string value) => "{0}={1}".FormatInvariant(EscapeValue(key), EscapeValue(value));

        private static string EscapeValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "%0%";
            }
            else
            {
                var length = System.Text.Encoding.UTF8.GetByteCount(value);
                return "%{0}%{1}".FormatInvariant(length, value);
            }
        }
        // private static string EscapeValue(string value) => value?.Replace("%", "[%]")?.Replace("\"", "[\"]")?.Replace("[", "\"[\"")?.Replace("]", "\"]\"") ?? string.Empty;

        /// <summary>
        /// Set a list of items (using the list separator, interprets escapes).
        /// </summary>
        public Task SetAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, value, options);

        /// <summary>
        /// Sets a dictionary of items.
        /// </summary>
        public new async Task SetAsync(IDictionary<string, string> values, ApiOptions? options = null)
        {
            await Api.SetPropertyAsync(PropertyName, values, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the value of specified key.
        /// </summary>
        public async Task<string?> GetAsync(string key, ApiOptions? options = null)
        {
            var values = await GetAsync(options).ConfigureAwait(false);
            if (values != null && values.TryGetValue(key, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Gets a dictionary containing all values.
        /// </summary>
        public override async Task<IDictionary<string, string>?> GetAsync(ApiOptions? options = null)
        {
            var values = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
            return ParseValue(values.Data) ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Append single item (does not interpret escapes).
        /// </summary>
        public Task AppendAsync(string key, string value, ApiOptions? options = null) => Api.ChangeListAsync(PropertyName, ListOptionOperation.Append, FormatKeyValue(key, value), options);

        /// <summary>
        /// Append 1 or more items (same syntax as Set).
        /// </summary>
        public Task AddAsync(string key, string value, ApiOptions? options = null) => Api.ChangeListAsync(PropertyName, ListOptionOperation.Add, FormatKeyValue(key, value), options);

        /// <summary>
        /// Delete item if present (does not interpret escapes).
        /// </summary>
        public Task RemoveAsync(string key, ApiOptions? options = null) => Api.ChangeListAsync(PropertyName, ListOptionOperation.Remove, key, options);
    }
}
