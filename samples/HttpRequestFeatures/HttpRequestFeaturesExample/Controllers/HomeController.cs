using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Azure.CognitiveServices.Personalization.Featurizers;

namespace HttpRequestFeaturesExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly static string serviceEndpoint = "http://localhost:5000";
        private readonly static string subscriptionKey = "";
        private static readonly string url = serviceEndpoint;

        // Initialize Personalization client
        PersonalizerClient client = InitializePersonalizationClient(url, subscriptionKey);

        public IActionResult Index()
        {
            HttpRequestFeatures httpRequestFeatures = GetHttpRequestFeaturesFromRequest(Request);

            ViewData["UserAgent"] = JsonConvert.SerializeObject(httpRequestFeatures, Formatting.Indented);

            Tuple<string, string> personalizationobj = callPersonalizationService(httpRequestFeatures);
            ViewData["Personalization Rank Request"] = personalizationobj.Item1;
            ViewData["Personalization Reward Request"] = personalizationobj.Item2;
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

        private Tuple<string, string> callPersonalizationService(HttpRequestFeatures httpRequestFeatures)
        {
            // Generate an ID to associate with the request.
            string eventId = Guid.NewGuid().ToString();

            // Get the actions list to choose from personalization with their features.
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

            // Exclude an action for personalization ranking. This action will be held at its current position.
            IList<string> excludeActions = new List<string> { "juice" };

            // Rank the actions
            var request = new RankRequest(actions, currentContext, excludeActions, eventId);
            RankResponse response = client.Rank(request);

            string rankjson = JsonConvert.SerializeObject(request, Formatting.Indented);
            string rewardjson = JsonConvert.SerializeObject(response, Formatting.Indented);
            return Tuple.Create(rankjson, rewardjson);
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
        /// Initializes the personalization client.
        /// </summary>
        /// <param name="url">Azure endpoint</param>
        /// <param name="serviceKey">subscription key</param>
        /// <returns>Personalization client instance</returns>
        private static PersonalizerClient InitializePersonalizationClient(string url, string serviceKey)
        {
            PersonalizerClient client = new PersonalizerClient(
                new ApiKeyServiceClientCredentials(serviceKey))
            { Endpoint = url };

            return client;
        }

        /// <summary>
        /// Creates personalization actions feature list.
        /// </summary>
        /// <returns>List of actions for personalization.</returns>
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
