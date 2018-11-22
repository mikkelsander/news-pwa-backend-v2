using System.Collections.Generic;

namespace PWANews.Client.Dto
{
    public class ArticlesResponseDTO
    {
        public string Status { get; set; }
        public List<ArticleDTO> Articles { get; set; }
    }
}
