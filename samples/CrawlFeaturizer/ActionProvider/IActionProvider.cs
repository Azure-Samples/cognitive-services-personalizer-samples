using CrawlFeaturizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrawlFeaturizer.ActionProvider
{
    interface IActionProvider
    {
        /// <summary>
        /// Crawls the given feed url and extracts actions by parsing the feed.
        /// </summary>
        /// <param name="feedUrl">RSS feed Url</param>
        /// <returns><see cref="CrawlAction"/> objects for items fetched from the RSS feed.</returns>
        Task<IEnumerable<CrawlAction>> GetActionsAsync(string feedUrl);
    }
}