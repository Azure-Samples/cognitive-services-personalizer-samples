<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizerDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace PersonalizerDemo.Controllers
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
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizerDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace PersonalizerDemo.Controllers
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
>>>>>>> 0c6154585d032d694fa01add2bd1463a35bf52d5
}