//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.CognitiveServices.Personalizer.Featurizers
{
    public class GeoIP
    {
        public string Country { get; set; }

        public int CountryConfidence { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public int CityConfidence { get; set; }

        public int MarketingCode { get; set; }

        private static Regex KeyValueRegex = new Regex("^([^=]+)=(.+)$", RegexOptions.Compiled);

        /// <summary>
        /// Parses X-FD-RevIP as specified by https://azfddocs.azurewebsites.net/references/protocol/xfdrevip/
        /// </summary>
        /// <param name="header">The header value. e.g. country=United States,iso=us,state=Washington,city=Redmond,zip=98052,tz=-8,dma=819,asn=3598,lat=47.681199999999997,long=-122.1207,countrycf=8,citycf=5</param>
        /// <param name="geoIP">The allocated geoIP structure.</param>
        /// <returns>True if parsed successful, false otherwise.</returns>
        public static bool TryParse(string header, out GeoIP geoIP)
        {
            geoIP = default(GeoIP);

            if (string.IsNullOrWhiteSpace(header))
                return false;

            var map = from entry in header.Split(',')
                      let match = KeyValueRegex.Match(entry)
                      where match.Success
                      select new
                      {
                          Key = match.Groups[1].Value,
                          Value = match.Groups[2].Value
                      };

            geoIP = new GeoIP();
            int intValue;
            foreach (var kv in map)
            {
                switch (kv.Key)
                {
                    case "country":
                        geoIP.Country = kv.Value;
                        break;
                    case "city":
                        geoIP.City = kv.Value;
                        break;
                    case "state":
                        geoIP.State = kv.Value;
                        break;
                    case "countrycf":
                        if (int.TryParse(kv.Value, out intValue))
                            geoIP.CountryConfidence = intValue;
                        break;
                    case "citycf":
                        if (int.TryParse(kv.Value, out intValue))
                            geoIP.CityConfidence = intValue;
                        break;
                    case "dma":
                        if (int.TryParse(kv.Value, out intValue))
                            geoIP.MarketingCode = intValue;
                        break;
                }
            }

            return true;
        }
    }
}
