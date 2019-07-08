using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerBusinessDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo
{
    public interface IPersonalizerService
    {
        RankResponse GetRecommendations(IList<object> context, bool useTextAnalytics = false);
        IList<Article> GetRankedArticles(IList<object> context, bool useTextAnalytics = false);
        void Reward(Reward reward);
    }
}
