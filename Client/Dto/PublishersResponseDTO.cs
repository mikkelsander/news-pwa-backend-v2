using System.Collections.Generic;

namespace PWANews.Client.Dto
{
    public class PublishersResponseDTO
    {
        public string Status { get; set; }
        public List<PublisherDTO> Sources { get; set; }
    }
}
