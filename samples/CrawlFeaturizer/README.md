The goal of this sample is to show how to use Personalization API with the crawl featurizer pipeline

# To try this sample
- Clone the Azure Personalization Service
```bash
git clone https://github.com/Azure/personalization-rl.git
```
- Navigate to quickstart\CrawlFeaturizer
- Open `CrawlFeaturizer.sln`

## Prerequisites
### Set up Personalization Service
- Navigate to [Getting Started with Personalization](https://github.com/Azure/personalization-rl/blob/master/docs/getting-started.md)

- Follow the instructions to get the endpoint and key to your Personalization Loop

- Replace `ApiKey` variable's value in `Programs.cs` with the loop `Key`

- Replace `ServiceEndpoint` variable's value in `Programs.cs` with the `Endpoint` url

### Set up Cognitive Services Text Analytics
- Navigate to [Microsoft Azure Cognitive Services](https://azure.microsoft.com/en-us/try/cognitive-services/).

- Click on `Language APIs` tab.

- Click on `Get API Key` button and select the account you want to sign in with.

- Replace `cognitiveTextAnalyticsEndpoint` variable's value in `Programs.cs` with one of the `Endpoints`

- Replace `CognitiveTextAnalyticsAPIKey ` variable's value in `Programs.cs` with one of the `Keys`


## Visual Studio
- Set `CrawlFeaturizer` as the Start Up project in Visual Studio

- Run the project (press `F5` key)

## Crawl Pipeline
The Crawl pipeline consists of 2 stages
1. Crawl a feed url and get all the items listed in the feed. These items are the `Actions` that will be ranked by Personalization API. This is exposed through the `IActionsProvider` interface.
2. Each `Action` is decorated with `Features` by using some `ActionFeaturizer` e.g Cognitive Services Text Analytics, Cognitive Services Vision. This functionality is exposed through the `IActionFeaturizer` interface. Once we have a set of actions with features, those actions can be ranked using the Personalization API.

## Sample Walkthrough
- 6 News Topics and their RSS feed urls are hardcoded in the program

- When the program starts, the RSSFeedActionProvider accesses each RSS Feed and creates a collection of CrawlActions for each of the news articles listed in the feed.

- Next, each CrawlAction is sent to the Cognitive Services Text Analytics endpoint to extract key word phrases and get a sentiment score. These along with the articles news topic and title are used as features for the article

- In the user interaction loop, the user is asked the `time of day` and `location` where he/she would be reading the article. All the articles then passed to the Personalization API endpoint for ranking along with the given user context.

- Once ranking is done the top ranked article is displayed to the user asked if he/she would read it.

- The user provided reward value (yes/no) is then passed to the Personalization API endpoint as reward.

- Backend `OnlineTrainer` learns user preferences by analysing the reward values for the user context and recommended article.

- Overtime the system learns the user's preferences and starts returning very accurate news article recommendations.


