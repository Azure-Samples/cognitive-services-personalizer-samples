using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalizerTravelAgencyDemo.Models;
using System.Collections.Generic;

namespace PersonalizerTravelAgencyDemo
{
    public static class ControllerExtensions
    {
        public static IList<object> CreatePersonalizerContext(this Controller controller, UserContext context, HttpRequest request)
        {
            var result = new List<object>
            {
                new {context.Referrer,context.Tournament, context.Device}
            };

            if (request != null)
            {
                var userAgent = new UserAgentInfo();
                userAgent.UseUserAgent(request.Headers["User-Agent"]);
                result.Add(userAgent);
            }

            return result;
        }
    }
}
