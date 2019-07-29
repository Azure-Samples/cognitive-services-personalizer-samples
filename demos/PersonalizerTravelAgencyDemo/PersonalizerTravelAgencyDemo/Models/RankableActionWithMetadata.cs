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
            Image = action.Image;
            
        }

        public string Title { get; set; }

        public string Image { get; set; }
    }
}