using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Featurizers;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using System;
using System.Collections.Generic;

namespace HttpRequestFeaturesExample.Controllers
{
    [Route("api/[controller]")]
    public class PersonalizerController : Controller
    {
        PersonalizerClient client;

        public PersonalizerController(PersonalizerClient personalizerClient)
        {
            this.client = personalizerClient;
        }

        /// <summary>
        /// Creates a RankRequest with user time of day, HTTP request features,
        /// and taste as the context and several different foods as the actions
        /// </summary>
        /// <returns>RankRequest with user info</returns>
        [HttpGet("GenerateRank")]
        public RankRequest GenerateRank()
        {
            string eventId = Guid.NewGuid().ToString();

            // Get the actions list to choose from personalizer with their features.
            IList<RankableAction> actions = GetActions();

            // Get context information from the user.
            HttpRequestFeatures httpRequestFeatures = GetHttpRequestFeaturesFromRequest(Request);
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
            return new RankRequest(actions, currentContext, excludeActions, eventId);
        }

        /// <summary>
        /// Sends a rank to the personalizer client
        /// </summary>
        /// <param name="rankRequest">The RankRequest to be sent</param>
        /// <returns>RankResponse from request</returns>
        [HttpPost("PostRank")]
        public RankResponse PostRank([FromBody]RankRequest rankRequest)
        {
            return client.Rank(rankRequest);
        }

        /// <summary>
        /// Creates a RewardRequest with the given value
        /// </summary>
        /// <param name="rewardValue">Value to assign to the reward</param>
        /// <returns>RewardRequest with value given</returns>
        [HttpPost("GenerateReward")]
        public RewardRequest GenerateReward([FromBody]double rewardValue)
        {
            return new RewardRequest(rewardValue);
        }

        /// <summary>
        /// Sends reward for event with eventId to the personalizer client
        /// </summary>
        /// <param name="eventId">Id of the event</param>
        /// <param name="rewardRequest">The RewardRequest to be sent</param>
        /// <returns>Status of POST request</returns>
        [HttpPost("PostReward/{eventId}")]
        public string PostReward([FromRoute]string eventId, [FromBody]RewardRequest rewardRequest)
        {
            try
            {
                client.Reward(eventId, rewardRequest);
            }
            catch(Exception e)
            {
                return e.ToString();
            }
            return "204: No content (Success!)";
        }

        /// <summary>
        /// Retrieve features from the http request
        /// </summary>
        /// <param name="httpRequest">User's HTTP request info</param>
        /// <returns>User's HTTP Request features</returns>
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

        /// <summary>
        /// Get users time of the day context.
        /// </summary>
        /// <returns>Time of day feature selected by the user.</returns>
        private string GetUsersTimeOfDay()
        {
            Random rnd = new Random();
            string[] timeOfDayFeatures = new string[] { "morning", "noon", "afternoon", "evening", "night", "midnight" };
            int timeIndex = rnd.Next(timeOfDayFeatures.Length);
            return timeOfDayFeatures[timeIndex];
        }

        /// <summary>
        /// Gets user food preference.
        /// </summary>
        /// <returns>Food taste feature selected by the user.</returns>
        private string GetUsersTastePreference()
        {
            Random rnd = new Random();
            string[] tasteFeatures = new string[] { "salty", "bitter", "sour", "savory", "sweet" };
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
                    new List<object>() { new { taste = "savory", spiceLevel = "medium" }, new { nutritionLevel = 5, cuisine = "italian" } }
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
                    new List<object>() { new { taste = "sour", spiceLevel = "low" }, new { nutritionLevel = 8 } }
                },

                new RankableAction
                {
                    Id = "popcorn",
                    Features =
                    new List<object>() { new { taste = "salty", spiceLevel = "none" }, new { nutritionLevel = 3 } }
                },

                new RankableAction
                {
                    Id = "coffee",
                    Features =
                    new List<object>() { new { taste = "bitter", spiceLevel = "none" }, new { nutritionLevel = 3 }, new { drink = true } }
                },

                new RankableAction
                {
                    Id = "soup",
                    Features =
                    new List<object>() { new { taste = "sour", spiceLevel = "high" }, new { nutritionLevel =  7} }
                }
            };

            return actions;
        }
    }
}
