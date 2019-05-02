using CrawlFeaturizer.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CrawlFeaturizer.Util
{
    public class RSSParser
    {
        private HttpClient client;

        /// <summary>
        /// Number of items collected from each RSS feed.
        /// </summary>
        public int ItemLimit { get; set; } = 5;

        internal RSSParser() => client = new HttpClient();

        /// <summary>
        /// Downloads content from an RSS feed url and parses each item along with its attributes.
        /// </summary>
        /// <param name="rssFeedUrl">RSS Feed Url</param>
        public async Task<IEnumerable<RSSParsedElement>> ParseAsync(string rssFeedUrl)
        {
            IEnumerable<RSSParsedElement> rssParsedElements = null;

            try
            {
                Console.WriteLine("Parsing RSS feed" + rssFeedUrl);

                string data = await client.GetStringAsync(rssFeedUrl);

                rssParsedElements = ParseRSS(data);
                Console.WriteLine($"Successfully parsed '{rssFeedUrl}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process '{rssFeedUrl}'", ex);
                throw ex;
            }

            return rssParsedElements;
        }

        /// <summary>
        /// Parses a RSS feed content and creates <see cref="RSSParsedElement"/> objects for each article.
        /// </summary>
        /// <param name="data">RSS feed url content</param>
        private IEnumerable<RSSParsedElement> ParseRSS(string data)
        {
            var rss = XDocument.Parse(data);

            const string parseFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";
            const string parseFormat2 = "ddd, dd MMM yyyy HH:mm:ss Z";

            var elements = from x in rss.DescendantNodes()
                .OfType<XElement>()
                .Where(a => a.Name == "item")
                .Select((elem, index) =>
                {
                    var pubDateStr = elem.Descendants("pubDate").FirstOrDefault()?.Value;
                    if (pubDateStr != null)
                        pubDateStr = pubDateStr.Trim();

                    if (!DateTime.TryParseExact(pubDateStr, parseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime pubDate))
                        if (!DateTime.TryParseExact(pubDateStr, parseFormat2, CultureInfo.InvariantCulture, DateTimeStyles.None, out pubDate))
                            pubDate = DateTime.UtcNow;

                    return new { elem, pubDate, index };
                })
                .OrderByDescending(elem => elem.pubDate)
                // limit the feed to avoid getting too many
                .Take(ItemLimit)
                // The order of the items allows customers to specify their base-line policy
                .OrderBy(elem => elem.index)
                .Select(x => x.elem)
                           let guid = x.Descendants("guid").FirstOrDefault()?.Value
                           let title = x.Descendants("title").FirstOrDefault()?.Value
                           let description = x.Descendants("description").FirstOrDefault()?.Value.Split("<div")[0]
                           select new RSSParsedElement
                           {
                               Guid = guid,
                               Title = title,
                               Description = description
                           };

            return elements;
        }
    }
}