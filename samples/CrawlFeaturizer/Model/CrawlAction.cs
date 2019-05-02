using Microsoft.Azure.CognitiveServices.Personalization.Models;
using Newtonsoft.Json.Linq;

namespace CrawlFeaturizer.Model
{
    public class CrawlAction : RankableAction
    {
        /// <summary>
        /// Stores metadata collected for the action during crawl.
        /// </summary>
        public JObject Metadata { get; set; }
    }
}
