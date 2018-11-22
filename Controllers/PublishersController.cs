using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.Data;
using PWANews.Models.ViewModels;

namespace PWANews.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly PWANewsDbContext _context;

        public PublishersController(PWANewsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublishers()
        {
            var publishers = await _context.Publishers.ToListAsync();

            var models = publishers.Select(publisher => new PublisherViewModel()
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Description = publisher.Description,
                Url = publisher.Url,
                Category = publisher.Category,
                Language = publisher.Language,
                Country = publisher.Country

            }).ToList();

            return Ok(new { models.Count, publishers = models });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublisher([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher == null)
            {
                return NotFound();
            }

            var model = new PublisherViewModel()
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Description = publisher.Description,
                Url = publisher.Url,
                Category = publisher.Category,
                Language = publisher.Language,
                Country = publisher.Country
            };

            return Ok(model);
        }
    }
}