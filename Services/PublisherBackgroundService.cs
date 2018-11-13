using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PWANews.Data;
using PWANews.Entities;
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
        private List<Publisher> existingPublishers;

        public PublisherBackgroundService(INewsClient newsClient, IServiceProvider serviceProvider, ILogger<PublisherBackgroundService> logger)
        {
            _client = newsClient;
            _provider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("executing publisher background task");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();

                    existingPublishers = context.Publishers.ToList();

                    _logger.LogDebug("fetching publishers");
                    var fetchedPublishers = await _client.GetPublishers();
               
                    fetchedPublishers.Take(40).ToList().ForEach(fetchedPublisher =>
                    {
                        UpdateOrInsert(fetchedPublisher, context);
                    });

                    _logger.LogDebug("saving publishers to database");
      
                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }

        }

        private void UpdateOrInsert(Publisher fetchedPublisher, PWANewsDbContext context)
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


