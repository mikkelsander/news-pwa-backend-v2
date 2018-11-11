using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PWANews.Data;
using PWANews.Entities;
using PWANews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWANews.Services
{
    public class ArticleBackgroundService : BackgroundService
    {
        private readonly INewsClient _client;
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public ArticleBackgroundService(INewsClient newsClient, IServiceProvider serviceProvider, ILogger<PublisherBackgroundService> logger)
        {
            _client = newsClient;
            _provider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("executing article background task");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();

                    var publishers = context.Publishers.ToList();

                    if (publishers.Count <= 0)
                    {
                        _logger.LogDebug("no publishers in database.. retrying in 30 seconds..");
                        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    }

                    var asyncNetworkRequestsDictionary = new Dictionary<Publisher, Task<List<ArticleDTO>>>();
                               
                    foreach (var publisher in publishers)
                    {
                        asyncNetworkRequestsDictionary.Add(publisher, _client.GetArticlesFromPublisher(publisher.ThirdPartyId).);
                    }

                    foreach (var entry in asyncNetworkRequestsDictionary)
                    {
                        var publisher = entry.Key;
                        var asyncRequest = entry.Value;

                        var articles = new List<Article>();
                        var articleDTOs = await asyncRequest;

                        foreach (var dto in articleDTOs)
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

                                ExpiresAt = "",
                                PublisherId = publisher.Id,
                                Publisher = publisher
                            });

                        }

                        _logger.LogDebug("adding articles to database");

                        context.AddRange(articles);
                        publisher.Articles = articles;
                    }

                    _logger.LogDebug("updating publishers");

                    context.UpdateRange(publishers);

                    await context.SaveChangesAsync();
                    await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
                 
                }
            }

        }
    }
}
