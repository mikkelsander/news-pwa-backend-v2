using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Models.DomainModels
{
    public class Subscription
    {
        [Required]
        public string PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
