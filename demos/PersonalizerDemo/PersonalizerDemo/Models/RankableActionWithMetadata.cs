using Microsoft.Azure.CognitiveServices.Personalization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerDemo.Models
{
    public class RankableActionWithMetadata : RankableAction
    {
        public RankableActionWithMetadata(Article article)
        {
            Id = article.Id;
            ImageName = article.ImageName;
            Title = article.Title;
            Features = new List<object>()
                {
                    new {article.PublishedDayAgo},
                    new {article.BreakingNews},
                    new {article.NewsSource},
                };


            if (!string.IsNullOrWhiteSpace(article.NewsLocation))
            {
                Features.Add(new { article.NewsLocation });
            }
        }

        public string Title { get; set; }

        public string ImageName { get; set; }
    }
}