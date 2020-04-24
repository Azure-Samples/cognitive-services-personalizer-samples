# About this sample
This sample extracts HTTP request features and adds them as context features to a Rank request that can be sent to an Azure Personalizer instance. After this request is sent, the user has the option to send a followup Reward request and then regenerate a new Rank request.

This is a [ASP.NET Core MVC with Knockout.js](https://docs.microsoft.com/en-us/aspnet/single-page-application/) solution. When run, it displays a single page with the Rank request, Reward request, and HTTP request features shown. Pressing the **Send Request** button above the Rank request body sends it to your Personalizer instance with HTTP information gathered from the featurizer as part of the context features. Once sent, the **Send Request** button above the Reward request body can be pressed in order to send a corresponding reward. At any time, you can press the **Generate Request** button to create a new Rank request.

# To try this sample

## Prerequisites

- [.NET Core 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1)
- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/), or [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)

## Set up the sample
- Clone the Azure Personalizer Samples repo.

    ```bash
    git clone https://github.com/Azure-Samples/cognitive-services-personalizer-samples.git
    ```

- Navigate to _samples/HttpRequestFeatures_.

- Open `HttpRequestFeaturesExample.sln`.

## Set up Azure Personalizer Service

- Create a Personalizer instance in the Azure portal.

- In the Azure portal, find the `ServiceEndpoint` and `PersonalizerApiKey` in the Quick start tab.
  Fill in the `ServiceEndpoint` in appsettings.json.
  Configure the `PersonalizerApiKey` as an [app secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) in one of the following ways:

    > If you are using the .NET Core CLI, you can use the `dotnet user-secrets set "PersonalizerApiKey" "<API Key>"` command.
    > If you are using Visual Studio, you can right-click the project and select the **Manage User Secrets** menu option to configure the Personalizer keys. By doing this, Visual Studio will open a `secrets.json` file where you can add the keys as follows:

```JSON
{
  "PersonalizerApiKey": "<your personalizer key here>",
}
```

## Run the sample

Build and run HttpRequestFeaturesExample. Press **F5** if using Visual Studio, or `dotnet build` then `dotnet run` if using .NET Core CLI. Through the UI, you will be able to send a Rank request and a Reward request and see their responses, as well as the http request features extracted from your environment.
