using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PWANews.Client.Dto;
using PWANews.Interfaces;
using PWANews.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PWANews.Client
{
    public class NewsClient : INewsClient
    {
        private HttpClient _client;
        private readonly string _apiKey;

        public NewsClient(HttpClient httpClient, IConfiguration configuration)
        {
            _apiKey = configuration["NEWS_API_KEY"];

            httpClient.BaseAddress = new Uri("https://newsapi.org");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("user-agent", "PWA News");
            httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _client = httpClient;
       
        }

        public async Task<List<Publisher>> GetPublishers()
        {
            var response = await _client.GetAsync("/v2/sources");

            var content = await response.Content?.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                // no content - I should do something
            }

            var publishersResponse = JsonConvert.DeserializeObject<PublishersResponseDTO>(content);

            if (publishersResponse.Status != "ok")
            {
                // something went wrong - I should do something
            }

            var publishers = new List<Publisher>();

            foreach (var dto in publishersResponse.Sources)
            {
                var publisher = new Publisher()
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    Url = dto.Url,
                    Category = dto.Category,
                    Language = dto.Language,
                    Country = dto.Country
                };

                publishers.Add(publisher);

            }

            return publishers;
        }

        public async Task<List<Article>> GetArticlesFromPublisher(string publisherId)
        {
            var response = await _client.GetAsync(string.Format("/v2/top-headlines?sources={0}", publisherId));

            response.EnsureSuccessStatusCode();

            var content = await response.Content?.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                // no content - I should do something
            }

            var articlesResponse = JsonConvert.DeserializeObject<ArticlesResponseDTO>(content);

            if (articlesResponse.Status != "ok")
            {
                // something went wrong - I should do something
            }

            var articles = new List<Article>();

            foreach (var dto in articlesResponse.Articles)
            {
                articles.Add(new Article()
                {
                    Title = dto.Title,
                    Author = dto.Author,
                    Description = dto.Description,
                    Url = dto.Url,
                    UrlToImage = dto.UrlToImage,
                    PublishedAt = dto.PublishedAt,
                    Content = dto.Content,

                    CreatedAt = DateTime.UtcNow,
                    PublisherId = publisherId,
                });
            }

            return articles;
        }

    }
}
