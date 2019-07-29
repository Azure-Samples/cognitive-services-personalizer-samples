using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerTravelAgencyDemo.Models;
using PersonalizerTravelAgencyDemo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalizerTravelAgencyDemo.Services
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

        public RankResponse GetRecommendations(IList<object> context)
        {
            var eventId = Guid.NewGuid().ToString();
            var actions = _actionsRepository.GetActions();

            var request = new RankRequest(actions, context, null, eventId);
            RankResponse response = _personalizerClient.Rank(request);
            return response;
        }

        public IList<Article> GetRankedArticles(IList<object> context)
        {
            var recommendations = GetRecommendations(context).Ranking.Select(x => x.Id).ToList();
            var articles = _articleRepository.GetArticles();

            return articles.OrderBy(article => recommendations.IndexOf(article.Id)).ToList();
        }

        public void Reward(Reward reward)
        {
            _personalizerClient.Reward(reward.EventId, new RewardRequest(reward.Value));
        }
    }
}
