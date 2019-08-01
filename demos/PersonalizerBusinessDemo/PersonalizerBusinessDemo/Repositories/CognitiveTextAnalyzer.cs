using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo.Repositories
{
    public class CognitiveTextAnalyzer
    {
        private readonly ITextAnalyticsClient textAnalyticsClient = null;

        /// <summary>
        /// Initializes <see cref="ITextAnalyticsClient"/> object using cognitive service subscription key and endpoint region.
        /// </summary>
        /// <param name="textAnalyticsClient">ITextAnalyticsClient client.</param>
        public CognitiveTextAnalyzer(ITextAnalyticsClient textAnalyticsClient)
        {
            this.textAnalyticsClient = textAnalyticsClient;
        }

        /// <summary>
        /// Gets key phrases for the passed input text.
        /// </summary>
        /// <param name="inputText">Text to analyze</param>
        public async Task<IList<string>> GetKeyPhrasesAsync(string inputText)
        {
            var response = await GetKeyPhrasesBatchAsync(new List<string> { inputText });
            return response.FirstOrDefault();
        }

        /// <summary>
        /// Gets sentiment score for the passed input text.
        /// </summary>
        /// <param name="inputText">Text to analyze</param>
        public async Task<double?> GetSentimentAsync(string inputText)
        {
            var response = await GetSentimentBatchAsync(new List<string> { inputText });
            return response.FirstOrDefault();
        }

        private async Task<IList<IList<string>>> GetKeyPhrasesBatchAsync(IList<string> batchInputText)
        {
            KeyPhraseBatchResult result = await textAnalyticsClient.KeyPhrasesAsync(
                new MultiLanguageBatchInput(
                    batchInputText.Select((text, index) => new MultiLanguageInput("en", $"{index + 1}", text)).ToList()
                    )
                );
            return result.Documents.Select(k => k.KeyPhrases).ToList();
        }

        private async Task<IList<double?>> GetSentimentBatchAsync(IList<string> batchInputText)
        {
            SentimentBatchResult result = await textAnalyticsClient.SentimentAsync(
                 new MultiLanguageBatchInput(
                    batchInputText.Select((text, index) => new MultiLanguageInput("en", $"{index + 1}", text)).ToList()
                    )
                );
            return result.Documents.Select(s => s.Score).ToList();
        }
    }
}