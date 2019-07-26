using PersonalizerTravelAgencyDemo.Models;
using System.Collections.Generic;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public interface IArticleRepository
    {
        IList<Article> GetArticles();
        Article GetArticle(string id);
    }
}
