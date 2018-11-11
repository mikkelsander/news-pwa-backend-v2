using Microsoft.AspNetCore.Mvc;
using PWANews.Entities;
using PWANews.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PWANews.Services
{
    public interface INewsClient
    {
        Task<List<PublisherDTO>> GetPublishers();
        Task<List<ArticleDTO>> GetArticlesFromPublisher(string publisherName);
    }
}