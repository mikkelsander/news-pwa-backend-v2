using PWANews.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Models
{
    public class ArticlesResponse
    {
        public string Status { get; set; }
        public List<ArticleDTO> Articles { get; set; }
        public int TotalResults { get; set; }

    }
}
