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
using Microsoft.AspNetCore.Http;

namespace PersonalizerBusinessDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IPersonalizerService _personalizerService;

        public HomeController(IArticleRepository articleRepository, IPersonalizerService personalizerService)
        {
            _articleRepository = articleRepository;
            _personalizerService = personalizerService;
        }

        public IActionResult Index()
        {
            var model = JsonConvert.DeserializeObject<PageConfigModel>(LoadJson("config/general.json"));

            return View(model);
        }

        private static string LoadJson(string jsonFile)
        {
            using (StreamReader r = new StreamReader(jsonFile))
            {
                return r.ReadToEnd();
            }
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
            var model = _articleRepository.GetArticle(id);
            ViewData["Title"] = model.Title;
            return View(model);
        }
    }
}
