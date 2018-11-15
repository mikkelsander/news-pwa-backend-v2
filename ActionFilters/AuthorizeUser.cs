using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PWANews.Data;
using PWANews.Entities;
using System;
using System.Linq;

namespace PWANews.ActionFilters
{
    public class AuthorizeUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext.RequestServices.GetService<PWANewsDbContext>();

            var headers = filterContext.HttpContext.Request.Headers;

            if (headers.ContainsKey("Authorization"))
            {
                var token = headers["Authorization"].ToString().Replace("Bearer ", "");

                var user = context.Users.Where(obj => obj.AuthenticationToken == token).FirstOrDefault();

                if (user == null || user.AuthenticationTokenIsExpired())
                {
                    filterContext.Result = new UnauthorizedResult();
                }
            }
            else
            {
                filterContext.Result = new UnauthorizedResult(); ;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}

