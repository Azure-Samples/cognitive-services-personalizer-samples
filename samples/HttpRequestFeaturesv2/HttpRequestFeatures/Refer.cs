//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.Azure.CognitiveServices.Personalizer.Featurizers
{
    public class Refer
    {
        [JsonProperty("referer", NullValueHandling = NullValueHandling.Ignore)]
        public string Referer { get; set; }
    }
}
