using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerDemo.Models
{
    public class Article
    {
        public string Id { get; set; }

        public int PublishedDayAgo { get; set; }

        public bool BreakingNews { get; set; }

        public string NewsLocation { get; set; }

        public string NewsSource { get; set; }

        public string Title { get; set; }

        public string ImageName { get; set; }

        public IList<string> Text { get; set; }

        public string Author { get; set; }

        public string AuthorLink { get; set; }
    }
}
