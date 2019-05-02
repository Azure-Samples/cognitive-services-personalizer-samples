//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Azure.CognitiveServices.Personalization.Featurizers
{
    public class Utils
    {
        public static bool? GetIsSyntheticFromRequest(HttpRequest httpRequest)
        {
            if (httpRequest != null)
            {
                return httpRequest.Headers.ContainsKey("X-DS-Synthetic");
            }
            return false;
        }

        /// <summary>
        /// Parses X-FD-RevIP as specified by https://azfddocs.azurewebsites.net/references/protocol/xfdrevip/
        /// </summary>
        public static GeoLocation GetGeoLocationFromRequest(HttpRequest httpRequest)
        {
            if (httpRequest != null)
            {
                GeoIP.TryParse(httpRequest.Headers["X-FD-RevIP"], out GeoIP geoIP);

                if (geoIP != null)
                {
                    GeoLocation geoLocation = new GeoLocation();
                    geoLocation.Use(geoIP);
                    return geoLocation;
                }
            }
            return null;
        }

        public static Refer GetRefererFromRequest(HttpRequest httpRequest)
        {
            if (httpRequest != null)
            {
                var referer = httpRequest.Headers["Referer"];
                if (!StringValues.IsNullOrEmpty(referer))
                {
                    return new Refer { Referer = referer.ToString() };
                }
            }
            return null;
        }

        public static UserAgentInfo GetUserAgentFromRequest(HttpRequest httpRequest)
        {
            if (httpRequest != null)
            {
                string userAgent = httpRequest.Headers["User-Agent"];

                if (!StringValues.IsNullOrEmpty(userAgent))
                {
                    UserAgentInfo userAgentInfo = new UserAgentInfo();
                    userAgentInfo.UseUserAgent(userAgent);
                    return userAgentInfo;
                }
            }
            return null;
        }
    }
}
