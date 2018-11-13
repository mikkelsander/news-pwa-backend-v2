using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Entities
{
    public class Subscription
    {
        public string PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
