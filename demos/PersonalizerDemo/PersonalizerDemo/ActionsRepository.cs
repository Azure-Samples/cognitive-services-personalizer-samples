using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Newtonsoft.Json;
using PersonalizerDemo.ActionFeaturizer;
using PersonalizerDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerDemo
{
    public class ActionsRepository : IActionsRepository
    {
        private IList<RankableActionWithMetadata> _actions = new List<RankableActionWithMetadata>();
        private IList<RankableActionWithMetadata> _actionsWithTextAnalytics = new List<RankableActionWithMetadata>();

        public ActionsRepository(IHostingEnvironment hostingEnvironment, IActionFeaturizer actionFeaturizer)
        {
            var fileProvider = hostingEnvironment.ContentRootFileProvider;
            var contents = fileProvider.GetDirectoryContents("articles");
            var articles = contents
                .Select(file => System.IO.File.ReadAllText(file.PhysicalPath))
                .Select(fileContent => JsonConvert.DeserializeObject<Models.Article>(fileContent));

            CreateRankableActions(articles, actionFeaturizer);
        }

        public IList<RankableAction> GetActions(bool useTextAnalytics)
        {
            return useTextAnalytics ? _actionsWithTextAnalytics.Cast<RankableAction>().ToList() : _actions.Cast<RankableAction>().ToList();
        }

        public IList<RankableActionWithMetadata> GetActionsWithMetadata(bool useTextAnalytics)
        {
            return useTextAnalytics ? _actionsWithTextAnalytics : _actions;
        }

        private void CreateRankableActions(IEnumerable<Article> articles, IActionFeaturizer actionFeaturizer)
        {
            foreach (var article in articles)
            {
                CreateRankableAction(article, actionFeaturizer).Wait();
            }

            _actions = _actions.OrderBy(a => a.Id).ToList();
            _actionsWithTextAnalytics = _actionsWithTextAnalytics.OrderBy(a => a.Id).ToList();
        }

        private async Task CreateRankableAction(Article article, IActionFeaturizer actionFeaturizer)
        {
            this._actions.Add(new RankableActionWithMetadata(article));

            var rankableAction = new RankableActionWithMetadata(article);
            var features = await actionFeaturizer.FeaturizeActionsAsync(article);
            foreach (var feature in features)
            {
                rankableAction.Features.Add(feature);
            }

            this._actionsWithTextAnalytics.Add(rankableAction);
        }
    }
}