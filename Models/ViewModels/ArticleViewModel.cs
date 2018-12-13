using System;

namespace PWANews.Models.ViewModels
{
    public class ArticleViewModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string PublisherId { get; set; }
        public string Description { get; set; }
        //public string Author { get; set; }
        //public string Content { get; set; }
    }
}
