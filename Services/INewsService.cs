using Microsoft.AspNetCore.Mvc;
using PWANews.Entities;
using PWANews.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PWANews.Services
{
    public interface INewsService
    {
        Task<List<PublisherDTO>> GetPublishers();
        Task<List<ArticleDTO>> GetArticlesFromPublisher(string publisherName);
    }
}