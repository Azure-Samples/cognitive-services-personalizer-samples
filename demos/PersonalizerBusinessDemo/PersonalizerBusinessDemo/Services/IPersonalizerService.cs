using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerBusinessDemo.Models;
using System.Collections.Generic;

namespace PersonalizerBusinessDemo.Services
{
    public interface IPersonalizerService
    {
        RankResponse GetRecommendations(IList<object> context, bool useTextAnalytics = false);
        IList<Article> GetRankedArticles(IList<object> context, bool useTextAnalytics = false);
        void Reward(Reward reward);
    }
}
