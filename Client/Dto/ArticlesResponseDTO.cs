using PWANews.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Client.Dto
{
    public class ArticlesResponseDTO
    {
        public string Status { get; set; }
        public List<ArticleDTO> Articles { get; set; }
    }
}
