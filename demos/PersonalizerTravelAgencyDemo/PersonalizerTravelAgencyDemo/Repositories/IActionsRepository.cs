using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerTravelAgencyDemo.Models;
using System.Collections.Generic;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public interface IActionsRepository
    {
        IList<RankableAction> GetActions(bool useTextAnalytics);

        IList<RankableActionWithMetadata> GetActionsWithMetadata(bool useTextAnalytics);
    }
}