using PWANews.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWANews.Interfaces
{
    public interface INewsClient
    {
        Task<List<Publisher>> GetPublishers();
        Task<List<Article>> GetArticlesFromPublisher(string publisherId);
    }
}