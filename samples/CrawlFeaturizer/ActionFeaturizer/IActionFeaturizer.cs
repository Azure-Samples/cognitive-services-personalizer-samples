using CrawlFeaturizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrawlFeaturizer.ActionFeaturizer
{
    internal interface IActionFeaturizer
    {
        /// <summary>
        /// Add features to provided <see cref="CrawlAction"/> objects.
        /// </summary>
        Task FeaturizeActionsAsync(IEnumerable<CrawlAction> actions);
    }
}