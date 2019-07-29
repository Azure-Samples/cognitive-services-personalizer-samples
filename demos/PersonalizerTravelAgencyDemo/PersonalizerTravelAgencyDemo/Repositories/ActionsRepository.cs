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

        public ActionsRepository(IActionRepository actionRepository)
        {
            var actions = actionRepository.GetActions();

            CreateRankableActions(actions);
        }

        public IList<RankableAction> GetActions()
        {
            return _actions.Cast<RankableAction>().ToList();
        }

        public IList<RankableActionWithMetadata> GetActionsWithMetadata()
        {
            return _actions;
        }

        private void CreateRankableActions(IEnumerable<Action> actions)
        {
            foreach (var action in actions)
            {
                CreateRankableAction(action).Wait();
            }

            _actions = _actions.OrderBy(a => a.Id).ToList();
            _actionsWithTextAnalytics = _actionsWithTextAnalytics.OrderBy(a => a.Id).ToList();
        }

        private async Task CreateRankableAction(Action action)
        {
            this._actions.Add(new RankableActionWithMetadata(action));
            var rankableAction = new RankableActionWithMetadata(action);

            this._actionsWithTextAnalytics.Add(rankableAction);
        }
    }
}