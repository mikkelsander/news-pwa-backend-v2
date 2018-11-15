
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWANews.Data;
using PWANews.InputModels;
using PWANews.Interfaces;
using PWANews.Services;

namespace PWANews.Controllers
{
    [Produces("application/json")]
    [Route("api/authenticate")]
    public class AuthenticationController : Controller
    {
        private readonly PWANewsDbContext _context;
        private readonly IUserAuthenticationService _authService;


        public AuthenticationController(PWANewsDbContext context, IUserAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] LoginCredentialsModel credentials)
        {
            var user = await _context.Users.SingleOrDefaultAsync(obj => obj.Email == credentials.Email);

            if(user == null || !_authService.AuthenticateUser(user, credentials.Password))
            {
                return Unauthorized();
            }

            _authService.SetOrRefreshAuthenticationToken(user);

            await _context.SaveChangesAsync();

            return Ok(new { token = user.AuthenticationToken });
        }
    }
}