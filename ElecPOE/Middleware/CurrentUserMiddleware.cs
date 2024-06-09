using ElecPOE.Models;
using ElecPOE.Utlility;

namespace ElecPOE.Middleware
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;
        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = GetUser();

            if (string.IsNullOrEmpty(user))
            {
                user = "No User";
            }
            else
            {
                 user = GetUser();
            }

            context.Session.SetString("CurrentUser", user);

            await _next(context);
        }

        private string GetUser()
        {
            return Helper.loggedInUser;   
        }

       
    }
}
