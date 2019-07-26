using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using PersonalizerTravelAgencyDemo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public class ActionsRepository : IActionsRepository
    {
        private IList<RankableActionWithMetadata> _actions = new List<RankableActionWithMetadata>();
        private IList<RankableActionWithMetadata> _actionsWithTextAnalytics = new List<RankableActionWithMetadata>();

        public ActionsRepository(IArticleRepository articleRepository)
        {
            var articles = articleRepository.GetArticles();

            CreateRankableActions(articles);
        }

        public IList<RankableAction> GetActions(bool useTextAnalytics)
        {
            return useTextAnalytics ? _actionsWithTextAnalytics.Cast<RankableAction>().ToList() : _actions.Cast<RankableAction>().ToList();
        }

        public IList<RankableActionWithMetadata> GetActionsWithMetadata(bool useTextAnalytics)
        {
            return useTextAnalytics ? _actionsWithTextAnalytics : _actions;
        }

        private void CreateRankableActions(IEnumerable<Article> articles)
        {
            foreach (var article in articles)
            {
                CreateRankableAction(article).Wait();
            }

            _actions = _actions.OrderBy(a => a.Id).ToList();
            _actionsWithTextAnalytics = _actionsWithTextAnalytics.OrderBy(a => a.Id).ToList();
        }

        private async Task CreateRankableAction(Article article)
        {
            this._actions.Add(new RankableActionWithMetadata(article));

            var rankableAction = new RankableActionWithMetadata(article);

            this._actionsWithTextAnalytics.Add(rankableAction);
        }
    }
}