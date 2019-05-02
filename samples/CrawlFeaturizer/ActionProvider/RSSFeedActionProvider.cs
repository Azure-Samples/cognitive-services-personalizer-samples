using CrawlFeaturizer.Model;
using CrawlFeaturizer.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlFeaturizer.ActionProvider
{
    public class RSSFeedActionProvider : IActionProvider
    {
        private RSSParser rssParser;

        internal RSSFeedActionProvider(RSSParser rssParser) => this.rssParser = rssParser;

        /// <summary>
        /// Uses RSSParser to crawl a RSS feed and extract articles.
        /// </summary>
        public async Task<IEnumerable<CrawlAction>> GetActionsAsync(string rssFeedUrl)
        {
            IEnumerable<RSSParsedElement> rssElements = await rssParser.ParseAsync(rssFeedUrl);
            return rssElements.Select(rssElement => ConvertToCrawlAction(rssElement)).Where(ca => ca.Id!=null);
        }

        /// <summary>
        /// Creates a <see cref="CrawlAction"/> object by extracting data from the provided RSSParsedElement object.
        /// </summary>
        private CrawlAction ConvertToCrawlAction(RSSParsedElement rssParsedElement)
        {
            return new CrawlAction
            {
                Id = rssParsedElement.Guid,
                Features = new List<object> { new { title = rssParsedElement.Title } },
                Metadata = JObject.FromObject(new { rssParsedElement.Title, rssParsedElement.Description })
            };
        }
    }
}