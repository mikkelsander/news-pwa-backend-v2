using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.ActionFilters;
using PWANews.Data;
using PWANews.Extensions;
using PWANews.InputModels;
using PWANews.Models.DomainModels;
using PWANews.Models.ViewModels;

namespace PWANews.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticateUser]
    public class ArticlesController : ControllerBase
    {
        private readonly PWANewsDbContext _context;

        public ArticlesController(PWANewsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetArticles([FromQuery] PaginationInputModel pagination )
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = (User)HttpContext.Items["user"];

            var articles = await _context.Subscriptions
                .Include(sub => sub.Publisher)
                .ThenInclude(pub => pub.Articles)
                .Where(sub => sub.UserId == user.Id)
                .SelectMany(sub => sub.Publisher.Articles)
                .OrderByDescending(article => article.PublishedAt)
                .Paginate(pagination)
                .ToListAsync();

            var models = articles.Select(article => new ArticleViewModel()
            {
                Title = article.Title,
                Url = article.Url,
                UrlToImage = article.UrlToImage,
                PublishedAt = article.PublishedAt,
                PublisherId = article.PublisherId,
                Description = article.Description
            }).ToList();

            return Ok( new { models.Count, articles = models } );
        }
    }
}