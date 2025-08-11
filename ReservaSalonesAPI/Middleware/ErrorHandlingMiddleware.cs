using System.Net;
using System.Text.Json;

namespace ReservaSalones.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Define el código de estado y el mensaje de error.
            var code = HttpStatusCode.InternalServerError; // 500
            var result = JsonSerializer.Serialize(new { error = "Ocurrió un error inesperado." });

            // Puedes personalizar la respuesta para tipos de excepciones específicos.
            if (exception is InvalidOperationException)
            {
                code = HttpStatusCode.Conflict; // 409
                result = JsonSerializer.Serialize(new { error = exception.Message });

            }
            else if (exception is FluentValidation.ValidationException)
            {
                result = exception.Message;
                code = HttpStatusCode.BadRequest;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}