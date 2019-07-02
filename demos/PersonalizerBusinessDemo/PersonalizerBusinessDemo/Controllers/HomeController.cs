<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonalizerDemo.Models;

namespace PersonalizerDemo.Controllers
{
    public class HomeController : Controller
    {
=======
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonalizerDemo.Models;

namespace PersonalizerDemo.Controllers
{
    public class HomeController : Controller
    {
>>>>>>> 0c6154585d032d694fa01add2bd1463a35bf52d5
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
<<<<<<< HEAD

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
=======

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
>>>>>>> 0c6154585d032d694fa01add2bd1463a35bf52d5
        }

        public IActionResult DefaultArticle()
        {
            return View("DefaultArticle");
        }

        public IActionResult Article(string id)
        {
            var fileProvider = _hostingEnvironment.ContentRootFileProvider;
            var articleFileInfo = fileProvider.GetFileInfo("articles/" + id + ".json");
            var articleContent = System.IO.File.ReadAllText(articleFileInfo.PhysicalPath);
            var model = JsonConvert.DeserializeObject<Article>(articleContent);
            ViewData["Title"] = model.Title;
            return View(model);
<<<<<<< HEAD
        }
    }
}
=======
        }
    }
}
>>>>>>> 0c6154585d032d694fa01add2bd1463a35bf52d5
