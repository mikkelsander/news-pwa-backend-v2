using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.ActionFilters;
using PWANews.Data;
using PWANews.Entities;

namespace PWANews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeUser]
    public class PublishersController : ControllerBase
    {
        private readonly PWANewsDbContext _context;

        public PublishersController(PWANewsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Publisher> GetPublishers()
        {
            return _context.Publishers;
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

            return Ok(publisher);
        }
    }
}