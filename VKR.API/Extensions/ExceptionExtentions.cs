using VKR.API.Middlewares;

namespace VKR.API.Extensions
{
    public static class ExceptionExtentions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }

    }
}
