using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Models
{
    public class PublisherDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
    }
}
