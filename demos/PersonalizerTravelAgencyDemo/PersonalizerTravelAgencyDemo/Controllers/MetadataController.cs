using PersonalizerTravelAgencyDemo.Models;
using Microsoft.AspNetCore.Mvc;
using PersonalizerTravelAgencyDemo.Repositories;

namespace PersonalizerTravelAgencyDemo.Controllers
{
    [Route("api/[controller]")]
    public class MetadataController : Controller
    {
        private readonly IRankableActionRepository _actionsRepository;

        public MetadataController(IRankableActionRepository actionsRepository)
        {
            _actionsRepository = actionsRepository;
        }

        [HttpGet("Actions")]
        public JsonResult Actions()
        {
            return new JsonResult(_actionsRepository.GetActionsWithMetadata());
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