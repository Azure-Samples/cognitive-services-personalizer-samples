// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using ChatbotExample.Model;
using ChatbotExample.ReinforcementLearning;
using ChatbotExample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatbotExample.Bots
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
    public class PersonalizerChatbot : ActivityHandler
    {
        /// <summary>
        /// LuisRecognizer wrapper for this demo that helps discern user intent.
        /// </summary>
        private readonly CoffeeRecognizer _coffeeRecognizer;

        /// <summary>
        /// <see cref="RLContextManager"/> object used in this demo.
        /// </summary>
        private readonly RLContextManager _rlFeaturesManager;

        /// <summary>
        /// Client used to rank suggestions for user/reward good suggestions.
        /// </summary>
        private readonly PersonalizerClient _personalizerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalizerChatbot"/> class.
        /// </summary>
        /// <param name="coffeeRecognizer">LuisRecognizer wrapper for this demo that helps discern user intent</param>
        /// <param name="rLContextManager"><see cref="RLContextManager"/> object used in this demo.</param>
        /// <param name="personalizerClient">Client used to rank suggestions for user/reward good suggestions</param>
        public PersonalizerChatbot(CoffeeRecognizer coffeeRecognizer, RLContextManager rLContextManager, PersonalizerClient personalizerClient)
        {
            _coffeeRecognizer = coffeeRecognizer;
            _rlFeaturesManager = rLContextManager;
            _personalizerClient = personalizerClient;
        }

        /// <summary>
        /// Every conversation turn for our Personalizer chatbot will call this method.
        /// There are no dialogs used, the sample only uses "single turn" processing,
        /// meaning a single request and response, with no stateful conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Check LUIS model
                var recognizerResult = await _coffeeRecognizer.RecognizeAsync(turnContext, cancellationToken);
                var topIntent = recognizerResult?.GetTopScoringIntent();
                if (topIntent != null && topIntent.HasValue && topIntent.Value.intent != "None")
                {
                    Intents intent = (Intents)Enum.Parse(typeof(Intents), topIntent.Value.intent);
                    switch (intent)
                    {
                        case Intents.ShowMenu:
                            await turnContext.SendActivityAsync($"Here is our menu: \n Coffee: {CoffeesMethods.DisplayCoffees()}\n Tea: {TeaMethods.DisplayTeas()}", cancellationToken: cancellationToken);
                            break;
                        case Intents.ChooseRank:
                            // Here we generate the event ID for this Rank.
                            var response = await ChooseRankAsync(turnContext, _rlFeaturesManager.GenerateEventId(), cancellationToken);
                            _rlFeaturesManager.CurrentPreference = response.Ranking;
                            await turnContext.SendActivityAsync($"How about {response.RewardActionId}?", cancellationToken: cancellationToken);
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
                                await turnContext.SendActivityAsync($"Not sure what you like. Did you ask for a suggestion?", cancellationToken: cancellationToken);
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
                                await turnContext.SendActivityAsync($"Not sure what you dislike. Did you ask for a suggestion?", cancellationToken: cancellationToken);
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
        /// <returns>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
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

        /// <summary>
        /// Sends a rank request to Personalizer using randomly selected weather/date as the context features
        /// and the available coffees and teas as the actions
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn.</param>
        /// <param name="eventId">EventId for this particular rank request</param>
        /// <param name="cancellationToken">Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the rank response from Personalizer.</returns>
        private async Task<RankResponse> ChooseRankAsync(ITurnContext turnContext, string eventId, CancellationToken cancellationToken)
        {
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
            var response = await _personalizerClient.RankAsync(request, cancellationToken);
            await turnContext.SendActivityAsync(
                $"===== DEBUG MESSAGE RETURN FROM RANK =====\n" +
                "This is what Rank returned:\n" +
                $"{JsonConvert.SerializeObject(response, Formatting.Indented)}\n",
                cancellationToken: cancellationToken);
            return response;
        }

        /// <summary>
        /// Sends a reward request to Personalizer
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn.</param>
        /// <param name="eventId">EventId for the rank call being rewarded</param>
        /// <param name="reward">Value of the reward that will be sent</param>
        /// <param name="cancellationToken">Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the reward response from Personalizer.</returns>
        private async Task RewardAsync(ITurnContext turnContext, string eventId, double reward, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(
                "===== DEBUG MESSAGE CALL REWARD =====\n" +
                "Calling Reward:\n" +
                $"eventId = {eventId}, reward = {reward}\n",
                cancellationToken: cancellationToken);

            await _personalizerClient.RewardAsync(eventId, new RewardRequest(reward), cancellationToken);
        }

        /// <summary>
        /// Sends a welcome message to the user explaining the chatbot
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn.</param>
        /// <param name="cancellationToken">Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private async Task SendResetMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(
                $"This is a simple chatbot example that illustrates how to use Personalizer.\n" +
                "The bot learns what coffee or tea order is preferred by customers given some context information (such as weather, temperature, and day of the week) and information about the user.", cancellationToken: cancellationToken);
            await turnContext.SendActivityAsync(
                "To use the bot, just follow the prompts.\n" +
                "To try out a new imaginary context, type \"Reset\" and a new one will be randomly generated.", cancellationToken: cancellationToken);
            await turnContext.SendActivityAsync(
                $"Welcome to the coffee bot, please tell me if you want to see the menu or get a coffee or tea suggestion for today. Once I've given you a suggestion, you can reply with 'like' or 'don't like'. It's {_rlFeaturesManager.RLFeatures.DayOfWeek} today and the weather is {_rlFeaturesManager.RLFeatures.Weather}.\n",
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a message to the user prompting them to get a new suggestion or regenerate the context
        /// </summary>A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn.</param>
        /// <param name="cancellationToken">Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
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
