﻿This sample shows how to integrate LUIS with a bot using ASP.Net Core 2. 

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Azure-Samples/cognitive-services-personalizer-samples.git
```
- [Optional] Update the `appsettings.json` file under `cognitive-services-personalizer-samples/samples/ChatbotExample/` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites

## Necessary resources
- Create both a Personalizer and a Language Understanding (LUIS) resource in the Azure portal. These will be used to power the reinforcement learning and NLP cababilities of the bot, respectively.
- NOTE: when creating a LUIS resource in the portal, you can choose between creating both a prediction and an authoring resource, or one or the other. Only an authoring resource is needed for this sample. More on the two [here](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys#azure-resources-for-luis)
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

# Running and using the bot

## Visual Studio
- Right click on the LuisBot project file > Properties > Debug and change the App URL to match the base URL of the endpoint listed in [nlp-with-luis.bot](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/samples/ChatbotExample/nlp-with-luis.bot)
- Navigate to the samples folder (`cognitive-services-personalizer-samples/samples/ChatbotExample/`) and open `LuisBot.csproj` in Visual Studio 
- Run the project (press `F5` key)

## Visual Studio Code
- Open the `launch.json`file and add the `--urls` argument to the `-args` parameter along with the base URL of the endpoint listed in [nlp-with-luis.bot](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/samples/ChatbotExample/nlp-with-luis.bot)
  For example: `"args": ["--urls","https://localhost:4034"]`
- Open `cognitive-services-personalizer-samples/samples/ChatbotExample/` sample folder
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

## Deploy this bot to Azure (optional)

Follow [this tutorial](https://aka.ms/bot-framework-emulator-publish-Azure) if you want to deploy your bot to Azure.
Additionally, if you would like to register your bot with Azure Bot Service, you can follow the steps at [this link](https://dev.botframework.com/bots/provision).

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)

