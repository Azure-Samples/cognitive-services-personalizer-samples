using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalizerBusinessDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo
{
    public static class ControllerExtensions
    {
        public static IList<object> CreatePersonalizerContext(this Controller controller, UserContext context, HttpRequest request)
        {
            var result = new List<object>
            {
                new {context.WeekDay,context.Profile, context.Tournament}
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
