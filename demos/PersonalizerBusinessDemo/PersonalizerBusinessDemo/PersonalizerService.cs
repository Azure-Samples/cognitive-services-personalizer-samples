using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerBusinessDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo
{
    public class PersonalizerService : IPersonalizerService
    {
        private readonly IPersonalizerClient _personalizerClient;
        private readonly IActionsRepository _actionsRepository;
        private readonly IArticleRepository _articleRepository;

        public PersonalizerService(IActionsRepository actionsRepository, IPersonalizerClient personalizerClient, IArticleRepository articleRepository)
        {
            _actionsRepository = actionsRepository;
            _personalizerClient = personalizerClient;
            _articleRepository = articleRepository;
        }

        public RankResponse GetRecommendations(IList<object> context, bool useTextAnalytics = false)
        {
            var eventId = Guid.NewGuid().ToString();
            var actions = _actionsRepository.GetActions(useTextAnalytics);

            var request = new RankRequest(actions, context, null, eventId);
            RankResponse response = _personalizerClient.Rank(request);
            return response;
        }

        public IList<Article> GetRankedArticles(IList<object> context, bool useTextAnalytics = false)
        {
            var recommendations = GetRecommendations(context, useTextAnalytics).Ranking.Select(x => x.Id).ToList();
            var articles = _articleRepository.GetArticles();

            return articles.OrderBy(article => recommendations.IndexOf(article.Id)).ToList();
        }

        public void Reward(Reward reward)
        {
            _personalizerClient.Reward(reward.EventId, new RewardRequest(reward.Value));
        }
    }
}
