using PWANews.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Models
{
    public class PublishersResponse
    {
        public string Status { get; set; }
        public List<PublisherDTO> Sources { get; set; }
    }
}
