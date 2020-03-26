# Personalizer

This sample shows how Personalizer can be integrated with a chat bot.

## Run Personalizer Chatbot

In order to run this example:

1. Read through [README-LuisBot.md](./README-LuisBot.md) to setup LuisBot
2. In [RLFeatures.cs](./ReinforcementLearning/RLFeatures.cs), modify HostName to a deployed Personalizer service; in [RLContextManager.cs](./ReinforcementLearning/RLContextManager.cs), modify SubscriptionKey to that service's
3. Follow the instructions in [README-LuisBot.md](./README-LuisBot.md) to run the bot

## Interact with Chatbot

The chatbot can understand human inputs by defined "Intents" and "utterances". Those definitions can be found in [coffeebot.json](./CognitiveModels/coffeebot.json).

For example, if you type "what do you suggest", Luis will be able to categorize it as the Intent - "ChooseRank".

## Intent Processing

In this example, Luis Intents will be processed in [LuisBot.cs](./LuisBot.cs) and then it will call the Ranking API to get a suggestion, shown below:

``` C#
public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
{
    if (turnContext.Activity.Type == ActivityTypes.Message)
    {
        var recognizerResult = await _services.LuisServices[LuisKey].RecognizeAsync(turnContext, cancellationToken);
        var topIntent = recognizerResult?.GetTopScoringIntent();
        if (topIntent != null && topIntent.HasValue && topIntent.Value.intent != "None")
        {
            Intents intent = (Intents)Enum.Parse(typeof(Intents), topIntent.Value.intent);
            switch (intent)
            {
                ...
                case Intents.ChooseRank:
                    var response = await ChooseRankAsync(turnContext, _rlFeaturesManager.GenerateEventId());
                    await turnContext.SendActivityAsync($"What about {response.RewardActionId}");
                    break;
                ...
            }
            ...
        }
        ...
    }
    ...
}
```
