//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Globalization;

namespace Microsoft.Azure.CognitiveServices.Personalizer.Featurizers
{
    public class GeoLocation
    {
        public void Use(GeoIP geoIP)
        {
            this.Country = geoIP.Country;
            this.CountryConfidence = geoIP.CountryConfidence.ToString(CultureInfo.InvariantCulture);
            this.State = geoIP.State;
            this.City = geoIP.City;
            this.CityConfidence = geoIP.CityConfidence.ToString(CultureInfo.InvariantCulture);
            this.MarketingCode = geoIP.MarketingCode.ToString(CultureInfo.InvariantCulture);
        }

        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty("_countrycf", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryConfidence { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

        [JsonProperty("_citycf", NullValueHandling = NullValueHandling.Ignore)]
        public string CityConfidence { get; set; }

        [JsonProperty("dma", NullValueHandling = NullValueHandling.Ignore)]
        public string MarketingCode { get; set; }
    }
}