using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using VKR.API.Exceptions;

namespace VKR.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(httpContext, HttpStatusCode.NotFound, ex);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != null)
                    httpContext.Response.StatusCode = (int)ex.StatusCode;

                await HandleExceptionAsync(httpContext, (HttpStatusCode)httpContext.Response.StatusCode, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex);
            }
        }

        private async static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, Exception exception)
        {
            var responce = context.Response;

            responce.ContentType = "application/json";
            responce.StatusCode = (int)statusCode; 


            string result = JsonSerializer.Serialize(
                new
                {
                    Message = exception.Message,
                    StatusCode = responce.StatusCode
                });

            await responce.WriteAsync(result);

        }
    }

}
