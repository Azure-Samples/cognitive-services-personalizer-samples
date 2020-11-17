using Azure;
using Azure.AI.TextAnalytics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrawlFeaturizer.Util
{
    public class CognitiveTextAnalyzer
    {
        private readonly TextAnalyticsClient textAnalyticsClient = null;

        /// <summary>
        /// Initializes <see cref="ITextAnalyticsClient"/> object using cognitive service subscription key and endpoint region.
        /// </summary>
        /// <param name="textAnalyticsClient">ITextAnalyticsClient client.</param>
        internal CognitiveTextAnalyzer(TextAnalyticsClient textAnalyticsClient)
        {
            this.textAnalyticsClient = textAnalyticsClient;
        }

        /// <summary>
        /// Gets key phrases for the passed document.
        /// </summary>
        /// <param name="document">Text to analyze</param>
        public async Task<KeyPhraseCollection> GetKeyPhrasesAsync(string document)
        {
            Response<KeyPhraseCollection> results = await textAnalyticsClient.ExtractKeyPhrasesAsync(document, "en");

            return results.Value;
        }

        /// <summary>
        /// Gets sentiment score for the passed document.
        /// </summary>
        /// <param name="document">Text to analyze</param>
        public async Task<double?> GetSentimentAsync(string document)
        {
            DocumentSentiment sentiment = await textAnalyticsClient.AnalyzeSentimentAsync(document, "en");

            return sentiment.ConfidenceScores.Neutral;
        }
    }
}
