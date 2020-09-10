using System;
using System.Collections.Generic;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Exposes the Metata sub-properties.
    /// </summary>
    public class MpvMetadataProperties
    {
        private readonly MpvApi _api;
        private readonly string _prefix;

        public MpvMetadataProperties(MpvApi api, string propertyName)
        {
            _api = api;
            _prefix = propertyName;
        }

        /// <summary>
        /// Metadata key/value pairs.
        /// </summary>
        public MpvPropertyRead<IDictionary<string, string>?> Metadata => _metadata ??= new MpvPropertyRead<IDictionary<string, string>?>(_api, _prefix);
        private MpvPropertyRead<IDictionary<string, string>?>? _metadata;

        /// <summary>
        /// Value of metadata entry 'key'.
        /// </summary>
        public MpvPropertyIndexRead<string, string?> MetadataByKey => _metadataByKey ??= new MpvPropertyIndexRead<string, string?>(_api, _prefix + "/by-key/{0}");
        private MpvPropertyIndexRead<string, string?>? _metadataByKey;

        /// <summary>
        /// Number of metadata entries.
        /// </summary>
        public MpvPropertyRead<int?> MetadataListCount => _metadataListCount ??= new MpvPropertyRead<int?>(_api, _prefix + "/list/count");
        private MpvPropertyRead<int?>? _metadataListCount;

        /// <summary>
        /// Key name of the Nth metadata entry. (The first entry is 0).
        /// </summary>
        public MpvPropertyIndexRead<int, string?> MetadataListKey => _metadataListKey ??= new MpvPropertyIndexRead<int, string?>(_api, _prefix + "/list/{0}/key");
        private MpvPropertyIndexRead<int, string?>? _metadataListKey;

        /// <summary>
        /// Value of the Nth metadata entry.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> MetadataListValue => _metadataListValue ??= new MpvPropertyIndexRead<int, string?>(_api, _prefix + "/list/{0}/value");
        private MpvPropertyIndexRead<int, string?>? _metadataListValue;
    }
}
