using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PersonalizerTravelAgencyDemo.Models;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private IList<Article> _articles;

        public ArticleRepository(IHostingEnvironment hostingEnvironment)
        {
            var fileProvider = hostingEnvironment.ContentRootFileProvider;
            var contents = fileProvider.GetDirectoryContents("articles");
            _articles = contents
                            .Select(file => System.IO.File.ReadAllText(file.PhysicalPath))
                            .Select(fileContent => JsonConvert.DeserializeObject<Article>(fileContent))
                            .Where(a => a.Enabled)
                            .ToList();
        }

        public Article GetArticle(string id)
        {
            return _articles.FirstOrDefault(article => article.Id == id);
        }

        public IList<Article> GetArticles()
        {
            return _articles.ToList();
        }
    }
}
