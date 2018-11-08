using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Content { get; set; }

        public string ExpiresAt { get; set; }

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }

    }
}
