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
    public class PublisherBackgroundService : BackgroundService
    {
        private readonly INewsClient _client;
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        public TimeSpan SleepingPeriod { get; set; } = TimeSpan.FromDays(1);
        private List<Publisher> existingPublishers;

        public PublisherBackgroundService(INewsClient newsClient, IServiceProvider serviceProvider, ILogger<PublisherBackgroundService> logger, IConfiguration configuration)
        {
            _client = newsClient;
            _provider = serviceProvider;
            _logger = logger;

            SleepingPeriod = TimeSpan.FromMinutes(double.Parse(configuration.GetSection("PublisherFetchService:SleepingPeriodMinutes").Value));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("** PUBLISHER FETCH SERVICE IS STARTING **");
                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();

                    existingPublishers = await context.Publishers.ToListAsync();

                    _logger.LogDebug("fetching publishers");
                    var fetchedPublishers = await _client.GetPublishers();
               
                    fetchedPublishers.ForEach(fetchedPublisher =>
                    {
                        AddOrUpdatePublisher(fetchedPublisher, context);
                    });

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
                }

                _logger.LogDebug("** PUBLISHER FETCH SERVICE IS FINISHED **");
                await Task.Delay(SleepingPeriod, stoppingToken);
            }

        }

        private void AddOrUpdatePublisher(Publisher fetchedPublisher, PWANewsDbContext context)
        {
            var publisher = existingPublishers.Find(x => x.Id == fetchedPublisher.Id);

            if (publisher != null)
            {
                publisher.Name = fetchedPublisher.Name;
                publisher.Description = fetchedPublisher.Description;
                publisher.Url = fetchedPublisher.Url;
                publisher.Category = fetchedPublisher.Category;
                publisher.Language = fetchedPublisher.Language;
                publisher.Country = fetchedPublisher.Country;

                _logger.LogDebug(string.Format("updating exisiting publisher: {0}", publisher.Name));
                context.Update(publisher);
            }
            else
            {
                _logger.LogDebug(string.Format("adding new publisher: {0} to database", fetchedPublisher.Name));
                context.Add(fetchedPublisher);
            }
        }

    }
}


