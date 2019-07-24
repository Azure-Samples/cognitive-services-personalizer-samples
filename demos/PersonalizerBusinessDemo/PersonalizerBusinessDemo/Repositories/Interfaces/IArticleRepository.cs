using PersonalizerBusinessDemo.Models;
using System.Collections.Generic;

namespace PersonalizerBusinessDemo
{
    public interface IArticleRepository
    {
        IList<Article> GetArticles();
        Article GetArticle(string id);
    }
}
