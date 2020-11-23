using Azure;
using Azure.AI.TextAnalytics;
using CrawlFeaturizer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlFeaturizer.ActionFeaturizer
{
    /// <summary>
    /// This class is a Cognitive Text Analytics implementation of the <see cref="IActionFeaturizer"/> interface.
    /// Its FeaturizeActionsAsync method uses Microsoft Azure Cognitive Services Text Analytics API to
    /// featurize a <see cref="CrawlAction"/>
    /// </summary>
    public class CognitiveTextAnalyticsFeaturizer : IActionFeaturizer
    {
        private readonly TextAnalyticsClient textAnalyticsClient = null;

        internal CognitiveTextAnalyticsFeaturizer(TextAnalyticsClient textAnalyticsClient)
        {
            this.textAnalyticsClient = textAnalyticsClient;
        }

        /// <summary>
        /// Iterates through all the provided <see cref="CrawlAction"/> objects and uses 
        /// Microsoft Azure Cognitive Services Text Analytics API to extract key phrases and sentiments for each action.
        /// </summary>
        public async Task FeaturizeActionsAsync(IEnumerable<CrawlAction> actions)
        {
            await Task.WhenAll(actions.Select(async a =>
            {
                Metadata metadata = a.Metadata.ToObject<Metadata>();
                string content = $"{metadata.Title ?? string.Empty} {metadata.Description ?? string.Empty}";

                // Get key phrases from the article title and description
                Response<KeyPhraseCollection> keyPhrases = await textAnalyticsClient.ExtractKeyPhrasesAsync(content);

                // Create a dictionary of key phrases (with a constant values) since at this time we do not support list of strings features.
                var keyPhrasesWithConstValues = keyPhrases.Value.ToDictionary(x => x, x => 1);
                a.Features.Add(new { keyPhrases = keyPhrasesWithConstValues });

                // Get sentiment score for the article
                DocumentSentiment sentiment = await textAnalyticsClient.AnalyzeSentimentAsync(content);
                a.Features.Add(new { sentiment.ConfidenceScores });
            }
            ));
        }

        /// <summary>
        /// Defines the metadata that is collected for a crawled item for cognitive text analysis.
        /// </summary>
        private class Metadata
        {
            public string Title { get; set; }

            public string Description { get; set; }
        }
    }
}