using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using VKR.API.Services;
using VKR.Common.Const;

namespace VKR.API.Middlewares
{
    public class TokenValidateMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuthService authService)
        {
            var isOk = true;
            var sessionIdString = context.User.Claims.FirstOrDefault(x=>x.Type==ClaimNames.SessionId)?.Value;
            if(Guid.TryParse(sessionIdString,out var sessionId))
            {
                var session = await authService.GetSessionById(sessionId);
                if (!session.IsActive || session==null)
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
