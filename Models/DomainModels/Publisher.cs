using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Models.DomainModels
{
    public class Publisher
    {

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string Country { get; set; }

        public ICollection<Article> Articles { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
