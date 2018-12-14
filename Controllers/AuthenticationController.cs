
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.Data;
using PWANews.InputModels;
using PWANews.Interfaces;
using PWANews.Models.ViewModels;

namespace PWANews.Controllers
{
    [Produces("application/json")]
    [Route("api/authenticate")]
    public class AuthenticationController : ControllerBase
    {
        private readonly PWANewsDbContext _context;
        private readonly IUserAuthenticationService _authService;

        public AuthenticationController(PWANewsDbContext context, IUserAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] UserInputModel input)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == input.Username);

            if (user == null || !_authService.AuthenticateUser(user, input.Password))
            {
                return Unauthorized();
            }

            _authService.SetOrRefreshAuthenticationToken(user);

            await _context.SaveChangesAsync();

            var model = new UserViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                AuthenticationToken = user.AuthenticationToken
            };

            return Ok(model);
        }
    }
}