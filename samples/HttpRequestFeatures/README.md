# About this sample
This sample extracts HTTP request features and adds them as context feature to a Rank requests that are sent to an Azure Personalizer instance.

# Run sample
1. Create a Personalizer instance in the Azure portal.
2. Fill in the `ApiKey` and `ServiceEndpoint` strings in appsettings.json. These can be found in your Cognitive Services Quick start tab in the Azure portal.
3. Build and run HttpRequestFeaturesExample. Through the UI, you will be able to send a Rank request and see the http request features extracted from your environment, as well as the response.
