using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizerDemo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PersonalizerDemo.Controllers
{
    [Route("api/[controller]")]
    public class PersonalizerController : Controller
    {
        private readonly IPersonalizerClient _client;
        private readonly IActionsRepository _actionsRepository;

        public PersonalizerController(IPersonalizerClient client, IActionsRepository actionsRepository)
        {
            _client = client;
            _actionsRepository = actionsRepository;
        }

        // POST api/Personalizer/Recommendation
        [HttpPost("Recommendation")]
        public JsonResult Recommendation([FromBody] UserContext context)
        {
            var currentContext = CreateContext(context, context.UseUserAgent ? Request : null);
            var eventId = Guid.NewGuid().ToString();
            var actions = _actionsRepository.GetActions(context.UseTextAnalytics);

            var request = new RankRequest(actions, currentContext, null, eventId);
            RankResponse response = _client.Rank(request);

            return new JsonResult(response);
        }

        // POST api/Personalizer/Reward
        [HttpPost("Reward")]
        public void Reward([FromBody] Reward reward)
        {
            _client.Reward(reward.EventId, new RewardRequest(reward.Value));
        }

        private IList<object> CreateContext(UserContext context, HttpRequest request)
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