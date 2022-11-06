using System.IdentityModel.Tokens.Jwt;
using VKR.API.Services;

namespace VKR.API.Middlewares
{
    public class TokenValidateMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UsersService usersService)
        {
            var isOk = true;
            var sessionIdString = context.User.Claims.FirstOrDefault(x=>x.Type=="sessionId")?.Value;
            if(Guid.TryParse(sessionIdString,out var sessionId))
            {
                var session = await usersService.GetSessionById(sessionId);
                if (!session.IsActive)
                {
                    isOk = false;
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                }

            }
            if (isOk)
            {
                await _next(context);
            }

        }


    }
}
