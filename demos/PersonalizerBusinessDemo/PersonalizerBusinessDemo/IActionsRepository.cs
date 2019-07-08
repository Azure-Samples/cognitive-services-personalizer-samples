using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerBusinessDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo
{
    public interface IActionsRepository
    {
        IList<RankableAction> GetActions(bool useTextAnalytics);

        IList<RankableActionWithMetadata> GetActionsWithMetadata(bool useTextAnalytics);
    }
}