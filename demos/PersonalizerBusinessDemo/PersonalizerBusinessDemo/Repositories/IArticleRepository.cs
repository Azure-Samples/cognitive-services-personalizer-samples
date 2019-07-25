using PersonalizerBusinessDemo.Models;
using System.Collections.Generic;

namespace PersonalizerBusinessDemo.Repositories
{
    public interface IArticleRepository
    {
        IList<Article> GetArticles();
        Article GetArticle(string id);
    }
}
