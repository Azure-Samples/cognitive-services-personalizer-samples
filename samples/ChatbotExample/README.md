# Chatbot with Cognitive Services Personalizer

This sample shows how two Cognitive Services, LUIS and Personalizer, can be integrated into a coffee recommendation chat bot using ASP.NET Core 2.

# Setting up the bot

## Download the sample
- Clone the samples repository
```bash
git clone https://github.com/Azure-Samples/cognitive-services-personalizer-samples.git
```
- Open [ChatbotSample.sln](./ChatbotExample.sln) in Visual Studio or VS Code

## Necessary Azure resources
- Create both a Personalizer and a Language Understanding (LUIS) resource in the Azure portal. These will be used to power the reinforcement learning and NLP cababilities of the bot, respectively.
- NOTE: when creating a LUIS resource in the portal, you can choose between creating a prediction resource, an authoring resource, or both. Only an authoring resource is needed for this sample. More on the two [here](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys#azure-resources-for-luis)

## Set up Personalizer
- In [appsettings.json](./appsettings.json), modify `PersonalizerServiceEndpoint` to be your resource's endpoint; this can be found in the quick start tab in the portal
- Configure the PersonalizerEndpointKey (can also be found in the quick start tab) as an app secret in one of the following ways:
    - If you are using VS Code, you can use the `dotnet user-secrets set "PersonalizerEndpointKey" "<Endpoint Key>"` command. 
    - If you are using Visual Studio, you can right-click the project and select the Manage User Secrets menu option to configure the Personalizer keys. By doing this, Visual Studio will open a secrets.json file where you can add the keys as follows:
        ``` Json
        {
          "PersonalizerEndpointKey": "<your endpoint key>",
        }
        ```

## Set up LUIS
- Navigate to [LUIS portal](https://www.luis.ai).

- Click the `Sign in` button.

- Click on `My Apps`.

- Select the LUIS authoring resource you created earlier.

- Click on the `Import new app` button.

- Click on the `Choose File` and select [coffeebot.json](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/samples/ChatbotExample/CognitiveModels/coffeebot.json) from the `cognitive-services-personalizer-samples/samples/ChatbotExample/CognitiveModels` folder.

- Click on the [`Train`](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-train) button on the top right of the page; this will train your app on the model inside of chatbot.json.

- Once this is done, press the [`Publish`](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-publish-app) button and select "Production". NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.

- Update [nlp-with-luis.bot](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/samples/ChatbotExample/nlp-with-luis.bot) file with your AppId, AuthoringKey, Region and Version. 
    You can find this information under the `Manage` tab.

    - The `AppID` can be found in "Application Information"
    - The `AuthoringKey` can be found in "Azure Resources" > "Authoring Resource" > "Primary key"
    - The `Region` is listed above the primary key (authoring key)
    - The `Version` is located next to the name of your app in the top left corner

    You will have something similar to this in the services section of your .bot file to run this sample:

    ```json
    {
        "type":"luis",
        "name":"<some name>",
        "appId":"<your app id>",
        "version":"<your version number>",
        "authoringKey":"<your authoring key>",
        "region":"<your region>",
        "id":"<some number>"
    },
    ```
- [Optional] Update the `appsettings.json` file under `cognitive-services-personalizer-samples/samples/ChatbotExample/` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Running and interacting with the bot

## Visual Studio
- Right click on the LuisBot project file > Properties > Debug and change the App URL to match the base URL of the endpoint listed in [nlp-with-luis.bot](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/samples/ChatbotExample/nlp-with-luis.bot)
- Run the project (press `F5` key)

## Visual Studio Code
- Open the `launch.json`file and add the `--urls` argument to the `-args` parameter along with the base URL of the endpoint listed in [nlp-with-luis.bot](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/samples/ChatbotExample/nlp-with-luis.bot)
  For example: `"args": ["--urls","https://localhost:4034"]`
- Bring up a terminal, navigate to `cognitive-services-personalizer-samples/samples/ChatbotExample/` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframeworkemulator) is a desktop application that allows bot developers to test and debug
their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `cognitive-services-personalizer-samples/samples/ChatbotExample/` folder
- Select `nlp-with-luis.bot` file

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

## Deploy this bot to Azure (optional)
Follow [this tutorial](https://aka.ms/bot-framework-emulator-publish-Azure) if you want to deploy your bot to Azure.
Additionally, if you would like to register your bot with Azure Bot Service, you can follow the steps at [this link](https://dev.botframework.com/bots/provision).

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)