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
                using(var scope = _provider.CreateScope())
                {
                    List<Publisher> publishers = new List<Publisher>();

                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();

                    _logger.LogDebug("fetching publishers");

                    var DTOList = await _client.GetPublishers();

                    foreach (var dto in DTOList)
                    {
                        var publisher = new Publisher()
                        {
                            ThirdPartyId = dto.Id,
                            Name = dto.Name,
                            Description = dto.Description,
                            Url = dto.Url,
                            Category = dto.Category,
                            Language = dto.Language,
                            Country = dto.Country
                        };

                        
                        publishers.Add(publisher);
                    }


                    _logger.LogDebug("saving publishers to database");

                    context.AddRange(publishers);

                    await context.SaveChangesAsync();
                }


                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }

        }
    }
}
