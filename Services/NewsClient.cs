using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PWANews.Entities;
using PWANews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PWANews.Services
{
    public class NewsClient : INewsClient
    {
        private HttpClient _client;
        private readonly string _apiKey;

        public NewsClient(HttpClient httpClient, IConfiguration configuration)
        {
            _apiKey = configuration["PWANews:NewsApiKey"];

            httpClient.BaseAddress = new Uri("https://newsapi.org");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("user-agent", "PWA News");
            httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        
            _client = httpClient;

        }


        public async Task<List<PublisherDTO>> GetPublishers()
        {

            var response = await _client.GetAsync("/v2/sources?");

            var content = await response.Content?.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
               // error
            }

            var publisherResponse = JsonConvert.DeserializeObject<PublishersResponse>(content);

            if(publisherResponse.Status != "ok")
            {
                //error
            }

            return publisherResponse.Sources;
        }

        public async Task<List<ArticleDTO>> GetArticlesFromPublisher(string publisherName)
        {
            var response = await _client.GetAsync(string.Format("/v2/top-headlines?sources={0}", publisherName));

            response.EnsureSuccessStatusCode();

            var content = await response.Content?.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                // error
            }

            var articlesResponse = JsonConvert.DeserializeObject<ArticlesResponse>(content);

            if (articlesResponse.Status != "ok")
            {
                //error
            }

            return articlesResponse.Articles;
        }



    }
}
