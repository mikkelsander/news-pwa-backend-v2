﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.ActionFilters;
using PWANews.Data;
using PWANews.Models.DomainModels;
using PWANews.Models.InputModels;
using PWANews.Models.ViewModels;

namespace PWANews.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticateUser]
    public class SubscriptionsController : ControllerBase
    {
        private readonly PWANewsDbContext _context;

        public SubscriptionsController(PWANewsDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetSubscriptions()
        {
            var user = (User)HttpContext.Items["user"];

            var subscriptions = _context.Subscriptions.Include(sub => sub.Publisher).Where(sub => sub.UserId == user.Id);
            var models = await subscriptions.Select(sub => new SubscriptionViewModel()
            {
                PublisherId = sub.Publisher.Id,
                PublisherName = sub.Publisher.Name,
                PublisherCategory = sub.Publisher.Category

            }).ToListAsync();

            return Ok(new { models.Count, subscriptions = models });
        }

        [HttpPost]
        public async Task<IActionResult> PostSubscription([FromBody] SubscriptionInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = (User)HttpContext.Items["user"];

            if(_context.Subscriptions.Any(sub => sub.PublisherId == input.PublisherId && sub.UserId == user.Id))
            {
                return Conflict();
            }
    
            var subscription = new Subscription()
            {
                PublisherId = input.PublisherId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);

            await _context.SaveChangesAsync();
          
            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        [HttpDelete("{publisherId}")]
        public async Task<IActionResult> DeleteSubscription([FromRoute] int publisherId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = (User)HttpContext.Items["user"];

            var subscription = await _context.Subscriptions.FindAsync(user.Id, publisherId);

            if (subscription == null)
            {
                return NotFound();
            }

            _context.Subscriptions.Remove(subscription);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}