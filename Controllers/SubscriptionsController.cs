using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.Data;
using PWANews.Entities;

namespace PWANews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly PWANewsDbContext _context;

        public SubscriptionsController(PWANewsDbContext context)
        {
            _context = context;
        }

        // GET: api/Subscriptions
        [HttpGet]
        public IEnumerable<Subscription> GetSubscriptions()
        {
            return _context.Subscriptions;
        }

        // GET: api/Subscriptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscription([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        // PUT: api/Subscriptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscription([FromRoute] int id, [FromBody] Subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subscription.UserId)
            {
                return BadRequest();
            }

            _context.Entry(subscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Subscriptions
        [HttpPost]
        public async Task<IActionResult> PostSubscription([FromBody] Subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Subscriptions.Add(subscription);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubscriptionExists(subscription.UserId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSubscription", new { id = subscription.UserId }, subscription);
        }

        // DELETE: api/Subscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return Ok(subscription);
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.UserId == id);
        }
    }
}