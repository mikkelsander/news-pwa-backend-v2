using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PWANews.Data;
using PWANews.Entities;
using System;
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

                    var testPublishers = publishers.Take(5);

                    var tasks = testPublishers.Select(publisher =>
                    _client.GetArticlesFromPublisher(publisher.Id));

                    var fetchedArticles = await Task.WhenAll(tasks);

                    foreach (var publisherArticleList in fetchedArticles)
                    {
                        try
                        {
                            _logger.LogDebug("ARTICLE LIST LOOP");
                            var publisher = context.Publishers.Find(publisherArticleList.First().PublisherId);
                            var existingPublisherArticleList = context.Articles.Where(article => article.PublisherId == publisher.Id).ToHashSet<Article>();

                            foreach (var article in publisherArticleList)
                            {
                                _logger.LogDebug("ARTICLE ITEM LOOP");

                                if (existingPublisherArticleList.Contains(article))
                                {
                                    _logger.LogDebug("UPDATE article: {0}", article.Title);
                                    //context.Update(article);
                                }
                                else
                                {
                                    _logger.LogDebug("INSERT article: {0}", article.Title);
                                    context.Add(article);
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.Message);
                        }

                        //_logger.LogDebug(listOfArticles.ToString());
                        //context.AddRange(publisherArticleList);

                    }

                    _logger.LogDebug("saving articles");

                    await context.SaveChangesAsync();
                    await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
                }
            }

        }
    }
}
