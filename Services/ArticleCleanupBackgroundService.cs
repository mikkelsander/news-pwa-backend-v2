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
        private readonly TimeSpan _articleTimeToLive = TimeSpan.FromMinutes(2);
        private readonly TimeSpan _sleepingPeriod = TimeSpan.FromMinutes(1);
        private bool firstBoot = true;


        public ArticleCleanupBackgroundService(IServiceProvider serviceProvider, ILogger<ArticleCleanupBackgroundService> logger)
        {
            _provider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
  
            while (!stoppingToken.IsCancellationRequested)
            {

                if(firstBoot)
                {
                    firstBoot = false;
                    await Task.Delay(_sleepingPeriod, stoppingToken);
                }

                _logger.LogDebug("** ARTICLE CLEANUP SERVICE IS STARTING **");

                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PWANewsDbContext>();

                    var articles = context.Articles;             

                    var currentTime = new DateTime();

                    var expiredArticles = articles.Where(article => (currentTime-article.CreatedAt).Milliseconds >= _articleTimeToLive.Milliseconds).ToList();

                    if(expiredArticles != null || expiredArticles.Count > 0)
                    {
                        expiredArticles.ForEach(article => _logger.LogDebug("DELETING article: {0}", article.Title));
                        context.RemoveRange(expiredArticles);
                    }

                    _logger.LogDebug("** ARTICLE CLEANUP SERVICE IS FINISHED **");
                    await context.SaveChangesAsync();
                    await Task.Delay(_sleepingPeriod, stoppingToken);
                }
            }


        }
    }
}
