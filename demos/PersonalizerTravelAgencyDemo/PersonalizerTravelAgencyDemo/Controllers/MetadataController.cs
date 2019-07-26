using PersonalizerTravelAgencyDemo.Models;
using Microsoft.AspNetCore.Mvc;
using PersonalizerTravelAgencyDemo.Repositories;

namespace PersonalizerTravelAgencyDemo.Controllers
{
    [Route("api/[controller]")]
    public class MetadataController : Controller
    {
        private readonly IActionsRepository _actionsRepository;

        public MetadataController(IActionsRepository actionsRepository)
        {
            _actionsRepository = actionsRepository;
        }

        [HttpGet("Actions")]
        public JsonResult Actions(bool useTextAnalytics)
        {
            return new JsonResult(_actionsRepository.GetActionsWithMetadata(useTextAnalytics));
        }

        [HttpGet("UserAgent")]
        public JsonResult UserAgent()
        {
            var userAgent = new UserAgentInfo();
            userAgent.UseUserAgent(Request.Headers["User-Agent"]);

            return new JsonResult(userAgent);
        }
    }
}