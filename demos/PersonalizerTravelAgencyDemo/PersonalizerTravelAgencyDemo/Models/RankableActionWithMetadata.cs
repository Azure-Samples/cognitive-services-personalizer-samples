using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PersonalizerTravelAgencyDemo.Models
{
    public class RankableActionWithMetadata : RankableAction
    {
        public RankableActionWithMetadata(Action action)
        {
            Id = action.Id;

            Features = new List<object>()
            {
                new {action.ButtonColor},
                new {action.Image},
                new {action.Layout},
                new {action.ToneFont},
            };

            
        }


    }
}