using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonalizerBusinessDemo.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using PersonalizerBusinessDemo.Repositories;
using PersonalizerBusinessDemo.Services;

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
            ViewData["siteConfig"] = JsonConvert.DeserializeObject<PageConfigModel>(LoadJson("config/general.json"));

            return View();
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

        public IActionResult Article(string id)
        {
            ViewData["siteConfig"] = JsonConvert.DeserializeObject<PageConfigModel>(LoadJson("config/general.json"));

            var model = _articleRepository.GetArticle(id);
            ViewData["Title"] = model.Title;
            return View(model);
        }
        public IActionResult HomeSite(string articleIds)
        {
            ViewData["siteConfig"] = JsonConvert.DeserializeObject<PageConfigModel>(LoadJson("config/general.json"));


            if (String.IsNullOrWhiteSpace(articleIds))
            {
                return View("HomeSite", new List<Article>());
            }

            var articles = _articleRepository.GetArticles();

            List<string> topArticlesIds = articleIds.Split(",").ToList();

            var topArticles = articles.Where(article => topArticlesIds.Contains(article.Id))
                                        .OrderBy(article => topArticlesIds.IndexOf(article.Id))
                                        //Max articles that fit in layout
                                        .Take(4)
                                        .ToList();

            return View("HomeSite", topArticles);
        }
    }
}
