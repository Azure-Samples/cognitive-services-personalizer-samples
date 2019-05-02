//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.Azure.CognitiveServices.Personalization.Featurizers
{
    public class HttpRequestFeatures
    {

        [JsonProperty("_synthetic", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSynthetic { get; set; }

        [JsonProperty("Geo", NullValueHandling = NullValueHandling.Ignore)]
        public GeoLocation GeoLocation { get; set; }

        [JsonProperty("MRefer", NullValueHandling = NullValueHandling.Ignore)]
        public Refer Refer { get; set; }

        [JsonProperty("OUserAgent", NullValueHandling = NullValueHandling.Ignore)]
        public UserAgentInfo UserAgent { get; set; }

        public HttpRequestFeatures()
        {
        
        }
    }
}
