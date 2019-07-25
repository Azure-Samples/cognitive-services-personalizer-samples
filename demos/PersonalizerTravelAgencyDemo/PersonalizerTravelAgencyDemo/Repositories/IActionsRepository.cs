using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerBusinessDemo.Models;
using System.Collections.Generic;

namespace PersonalizerBusinessDemo.Repositories
{
    public interface IActionsRepository
    {
        IList<RankableAction> GetActions(bool useTextAnalytics);

        IList<RankableActionWithMetadata> GetActionsWithMetadata(bool useTextAnalytics);
    }
}