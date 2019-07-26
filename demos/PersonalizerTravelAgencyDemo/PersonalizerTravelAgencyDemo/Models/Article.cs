using System.Collections.Generic;

namespace PersonalizerTravelAgencyDemo.Models
{
    public class Article
    {
        public Article()
        {
            Enabled = true;
        }

        public string Id { get; set; }

        public bool Enabled { get; set; }

        public string PublishedAgo { get; set; }

        public bool BreakingNews { get; set; }

        public string NewsLocation { get; set; }

        public string Source { get; set; }

        public string Title { get; set; }

        public string ImageName { get; set; }

        public IList<string> Text { get; set; }

        public string Author { get; set; }

        public string AuthorLink { get; set; }

        public bool HasVideo { get; set; }

        public bool? FeaturedByEditorial { get; set; }

        public string[] Tags { get; set; }

        public IDictionary<string, object> Entities { get; set; }
    }
}
