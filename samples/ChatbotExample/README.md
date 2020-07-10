# Chatbot with Cognitive Services Personalizer

This sample shows how two Cognitive Services, LUIS and Personalizer, can be integrated into a coffee recommendation chat bot using ASP.NET Core 3.1.

# Architecture overview

When a user starts up the chatbot, it sends them a welcome message with a randomly generated day of the week and the current weather status; within the chatbot implementation,
these two features are used as context features for Personalizer, i.e. Personalizer will inform its decisions on what drinks to recommend the user based on what it has experienced in
the past when under those same conditions. These features will be reset every time the bot is restarted or when the user prompts a reset. When the user replies back,
the bot uses LUIS's intent recognition capabilities to discern what the user wants; the intents this LUIS app has been trained to recognize
include showing the user a coffee and tea menu, asking the chatbot for a drink suggestion, approving or disproving of the suggestion, and resetting the context features. 
When a user expresses an intent that LUIS interprets as asking for a suggestion, the bot will gather the the context features mentioned above, as well as its list of drinks and their features,
and send that information to Personalizer as a rank request. Personalizer will reply back with the same list of drinks and their corresponding ranking 
(i.e. how well Personalizer thinks the drink suits the user and the current scenario based on the features of the drink); the first item in the list is the top ranked suggestion, and the bot will in turn suggest it to the user. 
The user can then reply back. If LUIS registers their intent as liking the suggestion, the bot will send a reward request to Personalizer with the reward set to 1, indicating a positive response.
Conversely, if LUIS interprets the user's reply as disliking the suggestion, the bot will send a reward request with the reward set to 0, indicating a negative response. 
Personalizer uses this feedback to update its model, which will affect how it responds in similar future scenarios.

# Setting up the bot

## Install Prereqs
- [ASP.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [Microsoft Bot Framework Emulator](https://aka.ms/botframeworkemulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

## Download the sample
- Clone the samples repository
```bash
git clone https://github.com/Azure-Samples/cognitive-services-personalizer-samples.git
```
- Open [ChatbotSample.sln](./ChatbotExample.sln) in Visual Studio or VS Code

## LUIS Migration
- If you have a Language Understanding (LUIS) resource already created that you'd like to use, make sure you follow the LUIS migration steps [here](https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/migration) if any of the scenarios listed apply.

## Necessary Azure resources
- Create both a Personalizer and a Language Understanding resource in the Azure portal. These will be used to power the reinforcement learning and NLP cababilities of the bot, respectively.
- NOTE: when creating a LUIS resource in the portal, you can choose between creating a prediction resource, an authoring resource, or both. You can opt to only use an authoring resource for this sample; however, the prediction key and endpoint that luis.ai will provide for you in this case can only be used for a limited number of requests. For a more robust app experience, you should create both an authoring and prediction resource. More on the two [here](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys#azure-resources-for-luis).

## Set up Personalizer
- In [appsettings.json](./appsettings.json), modify `PersonalizerServiceEndpoint` to be your resource's endpoint and `PersonalizerAPIKey` to be either one of your resource's keys; this can be found in the Keys and Endpoint tab in the portal under `Endpoint` and `Key1`/`Key2`, respectively.

## Set up LUIS
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on `My Apps`.
- Select the Subscription for your LUIS authoring resource and then the LUIS authoring resource itself.
- Click on the `New app for conversation` button.
- Click on `Import as JSON` and select [coffeebot.json](./CognitiveModels/coffeebot.json) from the `cognitive-services-personalizer-samples/samples/ChatbotExample/CognitiveModels` folder.

- Click on the [`Train`](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-train) button on the top right of the page; this will train your app on the model inside of chatbot.json.

- Once this is done, press the [`Publish`](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-publish-app) button and select "Production" (You can leave the Sentiment Analysis, Spell Check, and Speech Priming fields set to Off). NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.

- Update [appsettings.json](./appsettings.json) with your `LuisAppId`, `LuisAPIKey`, and `LuisAPIServiceEndpoint`, all of which can be found under the `Manage` tab for your resource:
    - `LuisAppId` can be found under `Settings`
    - If you created both a LUIS authoring and prediction resource:
        - Under `Azure Resources` > `Prediction Resources`, click `Add prediction resource`
        - Fill out the `Subscription` and `LUIS Resource` fields with your prediction resource, and click `Done`
    - `LuisAPIKey` can be found under `Azure Resources` > `Prediction Resources` listed as `Primary Key`
    - `LuisServiceEndpoint` can be found under the `Azure Resources` > `Prediction Resources` listed as `Endpoint URL`.  
- You should now have an `appsettings.json` that looks like this:
    ```json
    {
        "PersonalizerChatbot:" {
            "LuisAppId": "<your app ID>",
            "LuisAPIKey": "<your LUIS API key>",
            "LuisServiceEndpoint": "<your hostname, e.g. https://westus.api.cognitive.microsoft.com>",
            "PersonalizerServiceEndpoint": "<your Personalizer endpoint, e.g. https://myPersonalizerResource.cognitiveservices.azure.com/",
            "PersonalizerAPIKey": "<your Personalizer API key>"
        }
    },
    ```

# Running and interacting with the bot

## Visual Studio
- Press the `F5` key to run the project

## Visual Studio Code
- Bring up the terminal, navigate to `cognitive-services-personalizer-samples/samples/ChatbotExample/` folder.
- Type `dotnet run`.

## Connect to bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter \<your App URL\>/api/messages as the Bot URL. You can find your App URL by going to [launchsettings.json](./Properties/launchsettings.json) where it is listed as `applicationUrl`. The default for this sample is `http://localhost:3978`

## Interact with Chatbot

The chatbot can understand human inputs by defined "Intents" and "utterances". Those definitions can be found in [coffeebot.json](./CognitiveModels/coffeebot.json).
For example, if you type "what do you suggest", LUIS will be able to categorize it as the Intent - "ChooseRank".

## Intent Processing

In this example, LUIS Intents will be processed in [PersonalizerChatbot.cs](./Bots/PersonalizerChatbot.cs) and then it will call the Ranking API to get a suggestion, shown below:

``` C#
public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
{
    if (turnContext.Activity.Type == ActivityTypes.Message)
    {
        var recognizerResult = await await _coffeeRecognizer.RecognizeAsync(turnContext, cancellationToken);
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

## Deploy this bot to Azure (optional)
Follow [this tutorial](https://aka.ms/azuredeployment) if you want to deploy your bot to Azure.

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Personalizer Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/personalizer/)
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)