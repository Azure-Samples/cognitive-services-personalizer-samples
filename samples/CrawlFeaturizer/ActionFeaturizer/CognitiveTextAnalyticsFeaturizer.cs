using CrawlFeaturizer.Model;
using CrawlFeaturizer.Util;
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
        private CognitiveTextAnalyzer cognitiveTextAnalyzer = null;

        internal CognitiveTextAnalyticsFeaturizer(CognitiveTextAnalyzer cognitiveTextAnalyzer)
        {
            this.cognitiveTextAnalyzer = cognitiveTextAnalyzer;
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
                IReadOnlyCollection<string> keyPhrases = await cognitiveTextAnalyzer.GetKeyPhrasesAsync(content);

                // Create a dictionary of key phrases (with a constant values) since at this time we do not support list of strings features.
                var keyPhrasesWithConstValues = keyPhrases.ToDictionary(x => x, x => 1);
                a.Features.Add(new { keyPhrases = keyPhrasesWithConstValues });

                // Get sentiment score for the article
                double? sentiment = await cognitiveTextAnalyzer.GetSentimentAsync(content);
                a.Features.Add(new { sentiment });
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