using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Entities
{
    public class Publisher
    {

        public int Id { get; set; }
        public string ThirdPartyId { get; set; } //news.org id for this source
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }

        public ICollection<Article> Articles { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }

    }
}
