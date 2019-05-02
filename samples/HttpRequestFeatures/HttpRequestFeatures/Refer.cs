//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Microsoft.Azure.CognitiveServices.Personalization.Featurizers
{
    public class Refer
    {
        [JsonProperty("referer", NullValueHandling = NullValueHandling.Ignore)]
        public string Referer { get; set; }
    }
}
