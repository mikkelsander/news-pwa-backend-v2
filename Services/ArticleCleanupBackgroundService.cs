using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PWANews.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWANews.Services
{
    public class ArticleCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        public TimeSpan ArticleTimeToLive { get; set; }
        public TimeSpan SleepingPeriod { get; set; }
        private bool firstBoot = true;

        public ArticleCleanupBackgroundService(IServiceProvider serviceProvider, ILogger<ArticleCleanupBackgroundService> logger, IConfiguration configuration)
        {
            _provider = serviceProvider;
            _logger = logger;

            ArticleTimeToLive = TimeSpan.FromMinutes(double.Parse(configuration.GetSection("ArticleCleanupService:ArticleTimeToLiveMinutes").Value));
            SleepingPeriod = TimeSpan.FromMinutes(double.Parse(configuration.GetSection("ArticleCleanupService:SleepingPeriodMinutes").Value));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                if (firstBoot)
                {
                    firstBoot = false;
                    await Task.Delay(SleepingPeriod, stoppingToken);
                }

                _logger.LogDebug("** ARTICLE CLEANUP SERVICE IS STARTING **");

                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();

                    var expiredArticles = await context.Articles.Where(article => (DateTime.UtcNow - article.CreatedAt).TotalDays > ArticleTimeToLive.TotalDays).ToListAsync();

                    if (expiredArticles != null || expiredArticles.Count > 0)
                    {
                        expiredArticles.ForEach(article => _logger.LogDebug("DELETING article: {0}", article.Title));
                        context.RemoveRange(expiredArticles);
                    }

                    try
                    {
                        _logger.LogDebug("** ARTICLE CLEANUP SERVICE IS FINISHED **");
                        await context.SaveChangesAsync();
                    }

                    catch (Exception e)
                    {
                        _logger.LogDebug("Failed to save changes");
                        _logger.LogError(e.Message);
                        _logger.LogError(e.StackTrace);
                    }

                    await Task.Delay(SleepingPeriod, stoppingToken);
                }
            }



        }
    }
}
