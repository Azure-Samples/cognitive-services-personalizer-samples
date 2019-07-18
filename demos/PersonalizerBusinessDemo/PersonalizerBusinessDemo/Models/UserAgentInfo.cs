//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using UAParser;
using System;
using System.Collections.Generic;

namespace PersonalizerBusinessDemo.Models
{
    public class UserAgentInfo
    {
        private static Dictionary<string, string> OStoDeviceType = new Dictionary<string, string>
        {
            { "Android", "Android" },
            { "BlackBerry OS", "Mobile" },
            { "Chrome OS", "Tablet" },
            { "iOS", "iOS" },
            { "Linux", "Desktop" },
            { "Mac OS X", "Desktop" },
            { "Other", "Other" },
            { "Symbian OS", "Mobile" },
            { "Ubuntu", "Desktop" },
            { "Windows", "Desktop" },
            { "Windows 10", "Desktop" },
            { "Windows 7", "Desktop" },
            { "Windows 8", "Desktop" },
            { "Windows 8.1", "Desktop" },
            { "Windows Phone", "Mobile" },
            { "Windows RT", "Tablet" },
            { "Windows RT 8.1", "Tablet" },
            { "Windows Vista", "Desktop" },
            { "Windows XP", "Desktop" }
        };

        private static Parser UAParserParser = Parser.GetDefault(new ParserOptions
        {
            UseCompiledRegex = true
        });

        private string valOrNull(string val) => String.IsNullOrEmpty(val) ? null : val;

        public void UseUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return;

            this.UserAgent = userAgent;

            var clientInfo = UAParserParser.Parse(userAgent);

            this.DeviceBrand = valOrNull(clientInfo.Device.Brand);
            this.DeviceFamily = valOrNull(clientInfo.Device.Family);
            this.DeviceIsSpider = clientInfo.Device.IsSpider;
            this.DeviceModel = valOrNull(clientInfo.Device.Model);
            this.OSFamily = valOrNull(clientInfo.OS.Family);
            this.OSMajor = valOrNull(clientInfo.OS.Major);
            this.OSPatch = valOrNull(clientInfo.OS.Patch);
            this.OSPatchMinor = valOrNull(clientInfo.OS.PatchMinor);

            if ("iOS".Equals(this.OSFamily, StringComparison.Ordinal))
            {
                if (this.DeviceModel.StartsWith("iPhone"))
                    this.DeviceType = "Mobile";
                else
                    this.DeviceType = "Tablet";
            }
            else
            {
                string deviceType;
                OStoDeviceType.TryGetValue(this.OSFamily, out deviceType);
                this.DeviceType = valOrNull(deviceType);
            }
        }

        [JsonProperty("_ua", NullValueHandling = NullValueHandling.Ignore)]
        public string UserAgent { get; set; }

        [JsonProperty("_DeviceBrand", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceBrand { get; set; }

        [JsonProperty("_DeviceFamily", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceFamily { get; set; }

        [JsonProperty("_DeviceIsSpider", NullValueHandling = NullValueHandling.Ignore)]
        public bool DeviceIsSpider { get; set; }

        [JsonProperty("_DeviceModel", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceModel { get; set; }

        [JsonProperty("_OSFamily", NullValueHandling = NullValueHandling.Ignore)]
        public string OSFamily { get; set; }

        [JsonProperty("_OSMajor", NullValueHandling = NullValueHandling.Ignore)]
        public string OSMajor { get; set; }

        [JsonProperty("_OSMinor", NullValueHandling = NullValueHandling.Ignore)]
        public string OSMinor { get; set; }

        [JsonProperty("_OSPatch", NullValueHandling = NullValueHandling.Ignore)]
        public string OSPatch { get; set; }

        [JsonProperty("_OSPatchMinor", NullValueHandling = NullValueHandling.Ignore)]
        public string OSPatchMinor { get; set; }

        [JsonProperty("DeviceType", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceType { get; set; }
    }
}
