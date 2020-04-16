using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Featurizers;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HttpRequestFeaturesExample.Controllers
{
    public class HomeController : Controller
    {
        PersonalizerClient client;

        public HomeController(PersonalizerClient personalizerClient)
        {
            this.client = personalizerClient;
        }

        public IActionResult Index()
        {
            HttpRequestFeatures httpRequestFeatures = GetHttpRequestFeaturesFromRequest(Request);

            ViewData["UserAgent"] = JsonConvert.SerializeObject(httpRequestFeatures, Formatting.Indented);
            
            // Generate an ID to associate with the request.
            string eventId = Guid.NewGuid().ToString();
            ViewData["EventId"] = eventId;

            Tuple<string, string, string> personalizerRank = callPersonalizerRank(httpRequestFeatures, eventId);
            ViewData["Personalizer Rank Request"] = personalizerRank.Item1;
            ViewData["Personalizer Rank Response"] = personalizerRank.Item2;
            ViewData["Personalizer rewardActionId"] = personalizerRank.Item3;

            string personalizerReward = callPersonalizerReward(eventId);
            ViewData["Personalizer Reward Request"] = personalizerReward;

            return View();
        }

        /// <summary>
        /// Retrieve features from the http request
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        private HttpRequestFeatures GetHttpRequestFeaturesFromRequest(HttpRequest httpRequest)
        {
            HttpRequestFeatures httpRequestFeatures = new HttpRequestFeatures
            {
                IsSynthetic = Utils.GetIsSyntheticFromRequest(httpRequest),
                GeoLocation = Utils.GetGeoLocationFromRequest(httpRequest),
                Refer = Utils.GetRefererFromRequest(httpRequest),
                UserAgent = Utils.GetUserAgentFromRequest(httpRequest)
            };
            return httpRequestFeatures;
        }

        private Tuple<string, string, string> callPersonalizerRank(HttpRequestFeatures httpRequestFeatures, string eventId)
        {
            // Get the actions list to choose from personalizer with their features.
            IList<RankableAction> actions = GetActions();

            // Get context information from the user.
            string timeOfDayFeature = GetUsersTimeOfDay();
            string tasteFeature = GetUsersTastePreference();

            // Create current context from user specified data.
            IList<object> currentContext = new List<object>() {
                    new { time = timeOfDayFeature },
                    new { taste = tasteFeature },
                    new { httpRequestFeatures }
            };

            // Exclude an action for personalizer ranking. This action will be held at its current position.
            IList<string> excludeActions = new List<string> { "juice" };

            // Rank the actions
            var request = new RankRequest(actions, currentContext, excludeActions, eventId);
            RankResponse response = client.Rank(request);

            string requestJson = JsonConvert.SerializeObject(request, Formatting.Indented);
            string responseJson = JsonConvert.SerializeObject(response, Formatting.Indented);
            string rewardActionId = response.RewardActionId;

            return Tuple.Create(requestJson, responseJson, rewardActionId);
        }

        private string callPersonalizerReward(string eventId)
        {
            var request = new RewardRequest();
            string requestJson = JsonConvert.SerializeObject(request, Formatting.Indented);

            client.Reward(eventId, request);

            return requestJson;
        }

        /// <summary>
        /// Get users time of the day context.
        /// </summary>
        /// <returns>Time of day feature selected by the user.</returns>
        static string GetUsersTimeOfDay()
        {
            Random rnd = new Random();
            string[] timeOfDayFeatures = new string[] { "morning", "afternoon", "evening", "night" };
            int timeIndex = rnd.Next(timeOfDayFeatures.Length);
            return timeOfDayFeatures[timeIndex];
        }

        /// <summary>
        /// Gets user food preference.
        /// </summary>
        /// <returns>Food taste feature selected by the user.</returns>
        static string GetUsersTastePreference()
        {
            Random rnd = new Random();
            string[] tasteFeatures = new string[] { "salty", "sweet" };
            int tasteIndex = rnd.Next(tasteFeatures.Length);
            return tasteFeatures[tasteIndex];
        }

        /// <summary>
        /// Creates personalizer actions feature list.
        /// </summary>
        /// <returns>List of actions for personalizer.</returns>
        private IList<RankableAction> GetActions()
        {
            IList<RankableAction> actions = new List<RankableAction>
            {
                new RankableAction
                {
                    Id = "pasta",
                    Features =
                    new List<object>() { new { taste = "salty", spiceLevel = "medium" }, new { nutritionLevel = 5, cuisine = "italian" } }
                },

                new RankableAction
                {
                    Id = "ice cream",
                    Features =
                    new List<object>() { new { taste = "sweet", spiceLevel = "none" }, new { nutritionalLevel = 2 } }
                },

                new RankableAction
                {
                    Id = "juice",
                    Features =
                    new List<object>() { new { taste = "sweet", spiceLevel = "none" }, new { nutritionLevel = 5 }, new { drink = true } }
                },

                new RankableAction
                {
                    Id = "salad",
                    Features =
                    new List<object>() { new { taste = "salty", spiceLevel = "low" }, new { nutritionLevel = 8 } }
                }
            };

            return actions;
        }
    }
}
