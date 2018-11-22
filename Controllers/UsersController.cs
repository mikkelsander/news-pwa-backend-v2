using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.ActionFilters;
using PWANews.Data;
using PWANews.InputModels;
using PWANews.Interfaces;
using PWANews.Models.DomainModels;
using PWANews.Models.ViewModels;

namespace PWANews.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PWANewsDbContext _context;
        private readonly IUserAuthenticationService _authService;

        public UsersController(PWANewsDbContext context, IUserAuthenticationService authService )
        {
            _context = context;
            _authService = authService;
        }

        [AuthenticateUser]
        [HttpGet]
        public IActionResult GetUser()
        {
            var user = (User)HttpContext.Items["user"];

            var model = new UserViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                AuthenticationToken = user.AuthenticationToken
            };


            return Ok(model);
        }

        //doesn't require authentication
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(x => x.Username == input.Username))
            {
                return Conflict();
            }

            var user = new User()
            {
                Username = input.Username,
                Password = _authService.HashPassword(input.Password),
                CreatedAt = DateTime.UtcNow
            };

            _authService.SetOrRefreshAuthenticationToken(user);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var model = new UserViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                AuthenticationToken = user.AuthenticationToken
            };

            return  Created("", model);
        }

        [AuthenticateUser]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = (User)HttpContext.Items["user"];
      
            user.Username = input.Username;
            user.Password = _authService.HashPassword(input.Password);

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            var model = new UserViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                AuthenticationToken = user.AuthenticationToken
            };

            return Ok(model);
        }

        [AuthenticateUser]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var user = (User)HttpContext.Items["user"];   
            var subscriptions = _context.Subscriptions.Where(sub => sub.UserId == user.Id);

            _context.Users.Remove(user);
            _context.RemoveRange(subscriptions);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}