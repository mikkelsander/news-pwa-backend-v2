using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PWANews.Data;
using PWANews.Interfaces;
using PWANews.Models.DomainModels;
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
        public TimeSpan SleepingPeriod { get; set; } = TimeSpan.FromHours(12);
        private bool firstBoot = true;

        public ArticleFetchBackgroundService(INewsClient newsClient, IServiceProvider serviceProvider, ILogger<ArticleFetchBackgroundService> logger, IConfiguration configuration)
        {
            _client = newsClient;
            _provider = serviceProvider;
            _logger = logger;

            SleepingPeriod = TimeSpan.FromMinutes(double.Parse(configuration.GetSection("ArticleFetchService:SleepingPeriodMinutes").Value));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                if (firstBoot)
                {
                    firstBoot = false;
                    _logger.LogDebug("First boot. Wating for publisher table to be populated.. retrying in 15 seconds..");
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }

                _logger.LogDebug("** ARTICLE FETCH SERVICE IS STARTING **");

                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();

                    var publishers = await context.Publishers.ToListAsync();

                    var fetchedArticles = (await FetchArticles(publishers)).SelectMany(list => list).ToList();

                    var articlesInDatabase = await context.Articles.ToListAsync();

                    var newArticles = fetchedArticles.Except(articlesInDatabase).ToList();

                    context.AddRange(newArticles);

                    //newArticles.ForEach(article =>
                    //    _logger.LogDebug("INSERT article: {0}", article.Title)
                    //);

                    var existingArticles = fetchedArticles.Intersect(articlesInDatabase).ToList();

                    //should do work to update articles here

                    //context.UpdateRange(existingArticles);

                    //existingArticles.ForEach(article =>
                    //    _logger.LogDebug("UPDATE article: {0}", article.Title)
                    //);

                    try
                    {
                        await context.SaveChangesAsync();

                        _logger.LogDebug("Changes saved");
                    }
                    catch (Exception e)
                    {
                        _logger.LogDebug("Failed to save changes");

                        _logger.LogError(e.Message);
                        _logger.LogError(e.StackTrace);
                    }

                    _logger.LogDebug("** ARTICLE FETCH SERVICE IS FINISHED **");

                    await Task.Delay(SleepingPeriod, stoppingToken);
                }
            }

        }


        //private  void AddOrUpdate(List<Article> listOfArticles, PWANewsDbContext context)
        //{

        //    var articlesInDatabase = await context.Articles.ToListAsync();

        //    var newArticles = listOfArticles.Except(articlesInDatabase).ToList();

        //    context.AddRange(newArticles);

        //    //newArticles.ForEach(article =>
        //    //    _logger.LogDebug("INSERT article: {0}", article.Title)
        //    //);

        //    var existingArticles = listOfArticles.Intersect(articlesInDatabase).ToList();

        //    //should do work to update articles here

        //    //context.UpdateRange(existingArticles);

        //    //existingArticles.ForEach(article =>
        //    //    _logger.LogDebug("UPDATE article: {0}", article.Title)
        //    //);

        //}

        private async Task<List<Article>[]> FetchArticles(List<Publisher> publishers)
        {
            var testPublishers = publishers.Take(15);

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
