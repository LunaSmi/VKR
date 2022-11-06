using VKR.API.Middlewares;

namespace VKR.API.Extensions
{
    public static class TokenValidateExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidateMiddleware>();
        }
    }
}
