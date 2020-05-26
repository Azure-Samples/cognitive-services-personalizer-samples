using Azure;
using Azure.AI.TextAnalytics;
using CrawlFeaturizer.ActionFeaturizer;
using CrawlFeaturizer.ActionProvider;
using CrawlFeaturizer.Model;
using CrawlFeaturizer.Util;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrawlFeaturizer
{
    internal class Program
    {
        // The key specific to your personalization service instance; e.g. "0123456789abcdef0123456789ABCDEF"
        private const string ApiKey = "";

        // The endpoint specific to your personalization service instance; e.g. https://westus2.api.cognitive.microsoft.com/
        private const string ServiceEndpoint = "";

        // Cognitive Service TextAnalytics Endpoint
        private const string CognitiveTextAnalyticsEndpoint = "";

        // API Key for the Cognitive Service for Text Analytics. See this Azure CLI Link to get the key: https://docs.microsoft.com/en-us/cli/azure/cognitiveservices/account/keys?view=azure-cli-latest#az-cognitiveservices-account-keys-list-examples
        private const string CognitiveTextAnalyticsAPIKey = "";

        /// <summary>
        /// RSS feeds for different news topics
        /// </summary>
        private static readonly Dictionary<string, string> newsRSSFeeds = new Dictionary<string, string>()
        {
            { "World" , "http://rss.cnn.com/rss/cnn_world.rss" },
            { "Business", "http://rss.cnn.com/rss/money_latest.rss"},
            { "Technology", "http://rss.cnn.com/rss/cnn_tech.rss"},
            { "Health", "http://rss.cnn.com/rss/cnn_health.rss"},
            { "Entertainment", "http://rss.cnn.com/rss/cnn_showbiz.rss"},
            { "Travel", "http://rss.cnn.com/rss/cnn_travel.rss"}
        };

        private static void Main(string[] args)
        {
            int iteration = 1;
            bool runLoop = true;

            // Initialize Personalization client.
            PersonalizerClient client = InitializePersonalizationClient(ServiceEndpoint);

            // Initialize the RSS Feed actions provider
            IActionProvider actionProvider = new RSSFeedActionProvider(new RSSParser
            {
                // Number of items to fetch while crawling the RSS feed
                ItemLimit = 2
            });

            // Initialize the Cognitive Services TextAnalyticsClient for featurizing the crawled action articles
            TextAnalyticsClient textAnalyticsClient = new TextAnalyticsClient(new Uri(CognitiveTextAnalyticsEndpoint), new AzureKeyCredential(CognitiveTextAnalyticsAPIKey));

            // Initialize the Cognitive Text Analytics actions featurizer
            IActionFeaturizer actionFeaturizer = new CognitiveTextAnalyticsFeaturizer(new CognitiveTextAnalyzer(textAnalyticsClient));

            var newsActions = new List<RankableAction>();

            foreach (var newsTopic in newsRSSFeeds)
            {
                Console.WriteLine($"Fetching Actions for: {newsTopic.Key} from {newsTopic.Value}");

                IList<CrawlAction> crawlActions = actionProvider.GetActionsAsync(newsTopic.Value).Result.ToList();
                Console.WriteLine($"Fetched {crawlActions.Count} actions");

                actionFeaturizer.FeaturizeActionsAsync(crawlActions).Wait(10000);
                Console.WriteLine($"Featurized actions for {newsTopic.Key}");

                // Generate a rankable action for each crawlAction and add the news topic as additional feature
                newsActions.AddRange(crawlActions.Select(a =>
                {
                    a.Features.Add(new { topic = newsTopic.Key });
                    return (RankableAction)a;
                }).ToList());
            }

            do
            {
                Console.WriteLine("Iteration: " + iteration++);

                // Get context information from the user.
                string username = GetUserName();
                string timeOfDay = GetUsersTimeOfDay();
                string location = GetLocation();

                // Create current context from user specified data.
                IList<object> currentContext = new List<object>() {
                    new { username },
                    new { timeOfDay },
                    new { location }
                };

                // Id to associate with the request
                string eventId = Guid.NewGuid().ToString();

                // Rank the actions
                var request = new RankRequest(newsActions, currentContext, null, eventId);
                RankResponse response = client.Rank(request);

                var recommendedAction = newsActions.Where(a => a.Id.Equals(response.RewardActionId)).FirstOrDefault();

                Console.WriteLine("Personalization service thinks you would like to read: ");
                Console.WriteLine("Id: " + recommendedAction.Id);
                Console.WriteLine("Features : " + JsonConvert.SerializeObject(recommendedAction.Features, Formatting.Indented));
                Console.WriteLine("Do you like this article ?(y/n)");

                float reward = 0.0f;
                string answer = GetKey();
                if (answer == "Y")
                {
                    reward = 1;
                    Console.WriteLine("Great!");
                }
                else if (answer == "N")
                {
                    reward = 0;
                    Console.WriteLine("You didn't like the recommended news article.");
                }
                else
                {
                    Console.WriteLine("Entered choice is invalid. Service assumes that you didn't like the recommended news article.");
                }

                Console.WriteLine("Personalization service ranked the actions with the probabilities as below:");
                Console.WriteLine("{0, 10} {1, 0}", "Probability", "Id");
                var rankedResponses = response.Ranking.OrderByDescending(r => r.Probability);
                foreach (var rankedResponse in rankedResponses)
                {
                    Console.WriteLine("{0, 10} {1, 0}", rankedResponse.Probability, rankedResponse.Id);
                }

                // Send the reward for the action based on user response.
                client.Reward(response.EventId, new RewardRequest(reward));

                Console.WriteLine("Press q to break, any other key to continue:");
                runLoop = !(GetKey() == "Q");
            } while (runLoop);
        }

        /// <summary>
        /// Initializes the personalization client.
        /// </summary>
        /// <param name="url">Azure endpoint</param>
        /// <returns>Personalization client instance</returns>
        private static PersonalizerClient InitializePersonalizationClient(string url)
        {
            PersonalizerClient client = new PersonalizerClient(
                new ApiKeyServiceClientCredentials(ApiKey))
            { Endpoint = url };


            return client;
        }

        /// <summary>
        /// Get user's name.
        /// </summary>
        /// <returns>User's name.</returns>
        private static string GetUserName()
        {
            Console.WriteLine("What is your name?");
            string name = Console.ReadLine();
            Console.WriteLine($"Hi {name}");
            return name;
        }

        /// <summary>
        /// Get user's time of the day context.
        /// </summary>
        /// <returns>Time of day feature selected by the user.</returns>
        private static string GetUsersTimeOfDay()
        {
            string[] timeOfDayFeatures = new string[] { "morning", "afternoon", "evening", "night" };

            Console.WriteLine("What time of day is it (enter number)? 1. morning 2. afternoon 3. evening 4. night");
            if (!int.TryParse(GetKey(), out int timeIndex) || timeIndex < 1 || timeIndex > timeOfDayFeatures.Length)
            {
                Console.WriteLine("Entered value is invalid. Setting feature value to " + timeOfDayFeatures[0] + ".");
                timeIndex = 1;
            }
            return timeOfDayFeatures[timeIndex - 1];
        }

        /// <summary>
        /// Get user's reading location.
        /// </summary>
        /// <returns>Selected location.</returns>
        private static string GetLocation()
        {
            string[] location = new string[] { "At home", "At work", "Travelling", "Other" };

            var options = string.Join(" ", location.Select((x, i) => string.Format("{0}. {1}", i + 1, x)));
            Console.WriteLine($"Where do you want to read? {options}");
            if (!int.TryParse(GetKey(), out int locIndex) || locIndex < 1 || locIndex > location.Count())
            {
                Console.WriteLine("Entered value is invalid. Setting feature value to " + location[0] + ".");
                locIndex = 1;
            }
            return location[locIndex - 1];
        }

        private static string GetKey()
        {
            string key = Console.ReadKey().Key.ToString().Last().ToString().ToUpper();
            Console.WriteLine();
            return key;
        }
    }
}