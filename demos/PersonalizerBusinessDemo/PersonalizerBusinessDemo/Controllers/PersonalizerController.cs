using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizerBusinessDemo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PersonalizerBusinessDemo.Controllers
{
    [Route("api/[controller]")]
    public class PersonalizerController : Controller
    {
        private readonly IPersonalizerService _service;

        public PersonalizerController(IPersonalizerService service)
        {
            _service = service;
        }

        // POST api/Personalizer/Recommendation
        [HttpPost("Recommendation")]
        public JsonResult Recommendation([FromBody] UserContext context)
        {
            var currentContext = this.CreatePersonalizerContext(context, context.UseUserAgent ? Request : null);

            return new JsonResult(_service.GetRecommendations(currentContext, context.UseTextAnalytics));
        }

        // POST api/Personalizer/Reward
        [HttpPost("Reward")]
        public void Reward([FromBody] Reward reward)
        {
            _service.Reward(reward);
        }
    }
}