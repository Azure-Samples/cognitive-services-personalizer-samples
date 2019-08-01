using PersonalizerBusinessDemo.Models;
using PersonalizerBusinessDemo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo.Services.ActionFeaturizer
{
    /// <summary>
    /// This class is a Cognitive Text Analytics implementation of the <see cref="IActionFeaturizer"/> interface.
    /// Its FeaturizeActionsAsync method uses Microsoft Azure Cognitive Services Text Analytics API to
    /// featurize a <see cref="CrawlAction"/>
    /// </summary>
    public class CognitiveTextAnalyticsFeaturizer : IActionFeaturizer
    {
        private CognitiveTextAnalyzer cognitiveTextAnalyzer = null;

        public CognitiveTextAnalyticsFeaturizer(CognitiveTextAnalyzer cognitiveTextAnalyzer)
        {
            this.cognitiveTextAnalyzer = cognitiveTextAnalyzer;
        }

        /// <summary>
        /// Iterates through all the provided <see cref="Article"/> objects and uses 
        /// Microsoft Azure Cognitive Services Text Analytics API to extract key phrases and sentiments for each action.
        /// </summary>
        public async Task<List<Object>> FeaturizeActionsAsync(Article article)
        {
            var features = new List<Object>();
            string content = $"{article.Title ?? string.Empty} {article.Text.Aggregate("", (accum, next) => accum + next + " ") ?? string.Empty}";

            // Get key phrases from the article title and description
            IList<string> keyPhrases = await cognitiveTextAnalyzer.GetKeyPhrasesAsync(content);

            // Create a dictionary of key phrases (with a constant values) since at this time we do not support list of strings features.
            var keyPhrasesWithConstValues = keyPhrases.ToDictionary(x => x, x => 1);

            features.Add(new { keyPhrases = keyPhrasesWithConstValues });

            // Get sentiment score for the article
            double? sentiment = await cognitiveTextAnalyzer.GetSentimentAsync(content);
            features.Add(new { sentiment });

            return features;
        }
    }
}