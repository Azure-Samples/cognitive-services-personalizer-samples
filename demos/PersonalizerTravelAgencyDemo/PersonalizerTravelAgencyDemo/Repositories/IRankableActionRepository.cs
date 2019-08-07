using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerTravelAgencyDemo.Models;
using System.Collections.Generic;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public interface IRankableActionRepository
    {
        IList<RankableAction> GetActions();

        IList<RankableActionWithMetadata> GetActionsWithMetadata();
    }
}