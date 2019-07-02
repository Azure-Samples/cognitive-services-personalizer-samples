using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonalizerBusinessDemo.Models;
using System.IO;

namespace PersonalizerBusinessDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = JsonConvert.DeserializeObject<PageConfigModel>(LoadJson("config/buttonsLabels.json"));

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
        }

        private static string LoadJson(string jsonFile)
        {
            using (StreamReader r = new StreamReader(jsonFile))
            {
                return r.ReadToEnd();
            }
        }

    }
}
