// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LuisBot.Model;
using LuisBot.ReinforcementLearning;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// For each interaction from the user, an instance of this class is created and
    /// the OnTurnAsync method is called.
    /// This is a transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each <see cref="Activity"/> received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    /// <seealso cref="!:https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    /// <seealso cref="!:https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.ibot?view=botbuilder-dotnet-preview"/>
    public class LuisBot : IBot
    {
        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public static readonly string LuisKey = "coffeebot";

        /// <summary>
        /// Services configured from the ".bot" file.
        /// </summary>
        private readonly BotServices _services;

        /// <summary>
        /// <see cref="RLContextManager"/> object used in this demo.
        /// </summary>
        private readonly RLContextManager _rlFeaturesManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LuisBot"/> class.
        /// </summary>
        /// <param name="services">Services configured from the ".bot" file.</param>
        public LuisBot(BotServices services)
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            _rlFeaturesManager = services.RLContextManager;
            if (!_services.LuisServices.ContainsKey(LuisKey))
            {
                throw new ArgumentException($"Invalid configuration. Please check your '.bot' file for a LUIS service named '{LuisKey}'.");
            }
        }

        /// <summary>
        /// Every conversation turn for our LUIS Bot will call this method.
        /// There are no dialogs used, the sample only uses "single turn" processing,
        /// meaning a single request and response, with no stateful conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Check LUIS model
                var recognizerResult = await _services.LuisServices[LuisKey].RecognizeAsync(turnContext, cancellationToken);
                var topIntent = recognizerResult?.GetTopScoringIntent();
                if (topIntent != null && topIntent.HasValue && topIntent.Value.intent != "None")
                {
                    Intents intent = (Intents)Enum.Parse(typeof(Intents), topIntent.Value.intent);
                    switch (intent)
                    {
                        case Intents.ShowMenu:
                            await turnContext.SendActivityAsync($"Here is our menu: \n Coffee {CoffeesMethods.DisplayCoffees()}\n Tea {TeaMethods.DisplayTeas()}", cancellationToken: cancellationToken);
                            break;
                        case Intents.ChooseRank:
                            // Here we generate the event ID for this Rank.
                            var response = await ChooseRankAsync(turnContext, _rlFeaturesManager.GenerateEventId(), cancellationToken);
                            _rlFeaturesManager.CurrentPreference = response.Ranking;
                            await turnContext.SendActivityAsync($"What about {response.RewardActionId}?", cancellationToken: cancellationToken);
                            break;
                        case Intents.RewardLike:
                            if (!string.IsNullOrEmpty(_rlFeaturesManager.CurrentEventId))
                            {
                                await RewardAsync(turnContext, _rlFeaturesManager.CurrentEventId, 1, cancellationToken);
                                await turnContext.SendActivityAsync($"That's great! I'll keep learning your preferences over time.", cancellationToken: cancellationToken);
                                await SendByebyeMessageAsync(turnContext, cancellationToken);
                            }
                            else
                            {
                                await turnContext.SendActivityAsync($"Not sure what you like about. Did you ask a suggestion?", cancellationToken: cancellationToken);
                            }

                            break;
                        case Intents.RewardDislike:
                            if (!string.IsNullOrEmpty(_rlFeaturesManager.CurrentEventId))
                            {
                                await RewardAsync(turnContext, _rlFeaturesManager.CurrentEventId, 0, cancellationToken);
                                await turnContext.SendActivityAsync($"Oh well, maybe I'll guess better next time.", cancellationToken: cancellationToken);
                                await SendByebyeMessageAsync(turnContext, cancellationToken);
                            }
                            else
                            {
                                await turnContext.SendActivityAsync($"Not sure what you dislike about. Did you ask a suggestion?", cancellationToken: cancellationToken);
                            }

                            break;
                        case Intents.Reset:
                            _rlFeaturesManager.GenerateRLFeatures();
                            await SendResetMessageAsync(turnContext, cancellationToken);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var msg = @"No LUIS intents were found.
                            This sample is about identifying intents:
                            'ShowMenu'
                            'ChooseRank'
                            'RewardLike'
                            'RewardDislike'.
                            Try typing 'Show me the menu','What do you suggest','I like it','I don't like it'.";
                    await turnContext.SendActivityAsync(msg);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                _rlFeaturesManager.GenerateRLFeatures();

                // Send a welcome message to the user and tell them what actions they may perform to use this bot
                await SendWelcomeMessageAsync(turnContext, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await SendResetMessageAsync(turnContext, cancellationToken);
                }
            }
        }

        private async Task<RankResponse> ChooseRankAsync(ITurnContext turnContext, string eventId, CancellationToken cancellationToken)
        {
            var client = new PersonalizerClient(
                new ApiKeyServiceClientCredentials(_rlFeaturesManager.SubscriptionKey))
            { Endpoint = _rlFeaturesManager.RLFeatures.HostName.ToString() };

            IList<object> contextFeature = new List<object>
            {
                new { weather = _rlFeaturesManager.RLFeatures.Weather.ToString() },
                new { dayofweek = _rlFeaturesManager.RLFeatures.DayOfWeek.ToString() },
            };

            Random rand = new Random(DateTime.UtcNow.Millisecond);
            IList<RankableAction> actions = new List<RankableAction>();
            var coffees = Enum.GetValues(typeof(Coffees));
            var beansOrigin = Enum.GetValues(typeof(CoffeeBeansOrigin));
            var organic = Enum.GetValues(typeof(Organic));
            var roast = Enum.GetValues(typeof(CoffeeRoast));
            var teas = Enum.GetValues(typeof(Teas));

            foreach (var coffee in coffees)
            {
                actions.Add(new RankableAction
                {
                    Id = coffee.ToString(),
                    Features =
                    new List<object>()
                    {
                        new { BeansOrigin = beansOrigin.GetValue(rand.Next(0, beansOrigin.Length)).ToString() },
                        new { Organic = organic.GetValue(rand.Next(0, organic.Length)).ToString() },
                        new { Roast = roast.GetValue(rand.Next(0, roast.Length)).ToString() },
                    },
                });
            }

            foreach (var tea in teas)
            {
                actions.Add(new RankableAction
                {
                    Id = tea.ToString(),
                    Features =
                    new List<object>()
                    {
                        new { Organic = organic.GetValue(rand.Next(0, organic.Length)).ToString() },
                    },
                });
            }

            var request = new RankRequest(actions, contextFeature, null, eventId);
            await turnContext.SendActivityAsync(
                "===== DEBUG MESSAGE CALL TO RANK =====\n" +
                "This is what is getting sent to Rank:\n" +
                $"{JsonConvert.SerializeObject(request, Formatting.Indented)}\n",
                cancellationToken: cancellationToken);
            var response = await client.RankAsync(request, cancellationToken);
            await turnContext.SendActivityAsync(
                $"===== DEBUG MESSAGE RETURN FROM RANK =====\n" +
                "This is what Rank returned:\n" +
                $"{JsonConvert.SerializeObject(response, Formatting.Indented)}\n",
                cancellationToken: cancellationToken);
            return response;
        }

        private async Task RewardAsync(ITurnContext turnContext, string eventId, double reward, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(
                "===== DEBUG MESSAGE CALL REWARD =====\n" +
                "Calling Reward:\n" +
                $"eventId = {eventId}, reward = {reward}\n",
                cancellationToken: cancellationToken);

            var client = new PersonalizerClient(
                new ApiKeyServiceClientCredentials(_rlFeaturesManager.SubscriptionKey))
            { Endpoint = _rlFeaturesManager.RLFeatures.HostName.ToString() };
            await client.RewardAsync(eventId, new RewardRequest(reward), cancellationToken);
        }

        private async Task SendResetMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(
                $"This is a simple chatbot exmaple that illustrate how to use Cognitive services personalization.\n" +
                "The bot learns what coffee or tea order is preferred by customers given some context information (such as weather, temperature, and day of the week) and information about the user.", cancellationToken: cancellationToken);
            await turnContext.SendActivityAsync(
                "To use the bot, just follow the prompts.\n" +
                "To try out a new imaginary context, type \"Reset\" and a new one will be randomly generated.", cancellationToken: cancellationToken);
            await turnContext.SendActivityAsync(
                $"Welcome to the coffee bot, please tell me if you want to see the menu or get a coffee or tea suggestion for today. It's {_rlFeaturesManager.RLFeatures.DayOfWeek} today and the weather is {_rlFeaturesManager.RLFeatures.Weather}.\n",
                cancellationToken: cancellationToken);
        }

        private async Task SendByebyeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(
                $"Here were our attempted guesses for your preferences: " +
                JsonConvert.SerializeObject(_rlFeaturesManager.CurrentPreference.OrderByDescending(x => x.Probability), Formatting.Indented),
                cancellationToken: cancellationToken);
            await turnContext.SendActivityAsync(
                $"Would you like to get a new suggestion or reset the simulated context to a new day?",
                cancellationToken: cancellationToken);
        }
    }
}
