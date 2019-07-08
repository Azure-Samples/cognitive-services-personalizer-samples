using PersonalizerBusinessDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo
{
    public interface IArticleRepository
    {
        IList<Article> GetArticles();
        Article GetArticle(string id);
    }
}
