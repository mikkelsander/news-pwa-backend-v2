using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PWANews.Data;
using System.Linq;

namespace PWANews.ActionFilters
{
    public class AuthenticateUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {     
            
            var dbContext = filterContext.HttpContext.RequestServices.GetService<PWANewsDbContext>();

            var headers = filterContext.HttpContext.Request.Headers;

            if (headers.ContainsKey("Authorization"))
            {
                var token = headers["Authorization"].ToString().Replace("Bearer ", "");

                var user = dbContext.Users.Where(x => x.AuthenticationToken == token).FirstOrDefault();

                if (user == null || user.AuthenticationTokenIsExpired())
                {
                    filterContext.Result = new UnauthorizedResult();
                }

                filterContext.HttpContext.Items.Add("user", user);
            }
            else
            {
                filterContext.Result = new UnauthorizedResult(); ;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}

