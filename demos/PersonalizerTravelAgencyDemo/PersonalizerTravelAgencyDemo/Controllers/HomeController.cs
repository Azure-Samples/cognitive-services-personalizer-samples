using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonalizerTravelAgencyDemo.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using PersonalizerTravelAgencyDemo.Repositories;
using PersonalizerTravelAgencyDemo.Services;

namespace PersonalizerTravelAgencyDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActionRepository _actionRepository;
        private readonly IPersonalizerService _personalizerService;

        public HomeController(IActionRepository actionRepository, IPersonalizerService personalizerService)
        {
            _actionRepository = actionRepository;
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

        public IActionResult Action(string id)
        {
            ViewData["siteConfig"] = JsonConvert.DeserializeObject<PageConfigModel>(LoadJson("config/general.json"));

            var model = _actionRepository.GetAction(id);
            return View(model);
        }
        public IActionResult HomeSite(string actionsIds)
        {
            ViewData["siteConfig"] = JsonConvert.DeserializeObject<PageConfigModel>(LoadJson("config/general.json"));


            if (String.IsNullOrWhiteSpace(actionsIds))
            {
                return View("HomeSite", new List<Models.Action>());
            }

            var actions = _actionRepository.GetActions();

            List<string> topActionsIds = actionsIds.Split(",").ToList();

            var topActions = actions.Where(article => topActionsIds.Contains(article.Id))
                                        .OrderBy(article => topActionsIds.IndexOf(article.Id))
                                        //Max articles that fit in layout
                                        .Take(4)
                                        .ToList();

            return View("HomeSite", topActions);
        }
    }
}
