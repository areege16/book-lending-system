using BookLending.Application.Common.Responses;
using BookLending.Domain.Enums;

namespace BookLending.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception occurred");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = ResponseDto<object>.Error(ErrorType.UnexpectedError, "Something went wrong.");

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}