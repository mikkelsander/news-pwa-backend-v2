using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PWANews.Data;
using PWANews.Entities;
using PWANews.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWANews.Services
{
    public class ArticleFetchBackgroundService : BackgroundService
    {
        private readonly INewsClient _client;
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly TimeSpan _sleepingPeriod = TimeSpan.FromSeconds(60);
        private bool firstBoot = true;

        public ArticleFetchBackgroundService(INewsClient newsClient, IServiceProvider serviceProvider, ILogger<ArticleFetchBackgroundService> logger)
        {
            _client = newsClient;
            _provider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          

            while (!stoppingToken.IsCancellationRequested)
            {

                if (firstBoot)
                {
                    firstBoot = false;
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }

                _logger.LogDebug("** ARTICLE FETCH SERVICE IS STARTING **");

                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();
                    var publishers = context.Publishers.ToList();

                    if (publishers.Count <= 0 || publishers == null)
                    {
                        _logger.LogDebug("no publishers in database.. retrying in 30 seconds..");
                        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                        continue;
                    }

                    var arrayOfArticleLists = await FetchArticles(publishers);
           
                    foreach (var list in arrayOfArticleLists)
                    {
                        AddOrUpdate(list, context);
                    }

                    _logger.LogDebug("Saving changes");
                    await context.SaveChangesAsync();

                    _logger.LogDebug("** ARTICLE FETCH SERVICE IS FINISHED **");

                    await Task.Delay(_sleepingPeriod, stoppingToken);
                }
            }

        }

        private void AddOrUpdate(List<Article> list, PWANewsDbContext context)
        {
         
            try
            {                  
                var publisherId = list.First().PublisherId;
                var databaseList = context.Articles?.Where(article => article.PublisherId == publisherId).ToHashSet();

                // add new articles to database
                var newArticles = list.ToHashSet().Except(databaseList).ToList();                       
            
                if(newArticles != null && newArticles.Count > 0)
                {
                    newArticles.ForEach(article => _logger.LogDebug("INSERT article: {0}", article.Title));
                    context.AddRange(newArticles);
                }
         
                //update already existing articles
                var existingArticles = list.ToHashSet().Intersect(databaseList).ToList();

                if(existingArticles != null && existingArticles.Count > 0)
                {
                    existingArticles.ForEach(article =>

                    _logger.LogDebug("UPDATE article: {0}", article.Title)

                    //should do work to update articles
                    //context.UpdateRange(existingArticles);

                    );             
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
            }
        }


        private async Task<List<Article>[]> FetchArticles(List<Publisher> publishers)
        {
            var testPublishers = publishers.Take(5);

            try
            {
                var requests = testPublishers.Select(publisher =>
                _client.GetArticlesFromPublisher(publisher.Id));
                return await Task.WhenAll(requests);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return null;
            }
        }       
    }
}
