using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    public static class MpvResponseExtensions
    {
        /// <summary>
        /// Shortcut to access MpvResponse.Data when it has a value.
        /// </summary>
        public static T Value<T>(this MpvResponse<T?> response)
            where T : class
        {
            response.CheckNotNull(nameof(response));
            response.Data.CheckNotNull(nameof(response.Data));

            return response.Data!;
        }

        /// <summary>
        /// Shortcut to access MpvResponse.Data.Value when it has a value.
        /// </summary>
        public static T Value<T>(this MpvResponse<T?> response)
            where T : struct
        {
            response.CheckNotNull(nameof(response));
            response.Data.CheckNotNull(nameof(response.Data));

            return response.Data!.Value;
        }

        /// <summary>
        /// Returns a copy of the MpvResponse object while changing the data type and setting specified data.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <param name="response">The response to copy and change type.</param>
        /// <param name="data">The data to set.</param>
        /// <returns>A MpvResponse object of specified type.</returns>
        public static MpvResponse<T>? Copy<T>(this MpvResponse? response, [AllowNull] T data)
        {
            if (response != null)
            {
                return new MpvResponse<T>()
                {
                    Data = data,
                    Error = response.Error,
                    RequestID = response.RequestID
                };
            }
            return null;
        }

        /// <summary>
        /// Returns a copy of the MpvResponse object while parsing data into specified type.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <param name="response">The response to copy and change type.</param>
        /// <param name="data">The data to parse.</param>
        /// <returns>A MpvResponse object of specified type.</returns>
        public static MpvResponse<T>? Parse<T>(this MpvResponse? response)
        {
            return response.Copy(ParseData<T>(response));
        }

        [return: MaybeNull]
        public static T ParseData<T>(this MpvResponse? response)
        {
            var data = response?.Data;
            if (data == null) { return default; }

            if (typeof(T) == typeof(string))
            {
                var str = data.ToString();
                if (str.Length >= 2 && str[0] == '"' && str[str.Length - 1] == '"')
                {
                    str = str.Substring(1, str.Length - 2);
                }
                return (T)(object)str;
            }
            if (typeof(T).IsValueType)
            {
                var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)Convert.ChangeType(data, type, CultureInfo.InvariantCulture);
            }
            else
            {
                var jsonOptions = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = new MpvJsonNamingPolicy()
                };
                return JsonSerializer.Deserialize<T>(data, jsonOptions);
            }
        }

        ///// <summary>
        ///// Creates a copy of MpvResponse to contain parsed typed data.
        ///// </summary>
        ///// <typeparam name="T">The type of data to parse.</typeparam>
        ///// <param name="response">The response to parse.</param>
        ///// <returns>A copy of MpvResponse, or null.</returns>
        //public static MpvResponse<T>? Parse<T>(this MpvResponse? response)
        //{
        //    if (response != null)
        //    {
        //        var data = ParseData<T>(response.Data);

        //        return new MpvResponse<T>()
        //        {
        //            Data = data!,
        //            Error = response.Error,
        //            RequestID = response.RequestID
        //        };
        //    }
        //    return null;
        //}
    }
}
