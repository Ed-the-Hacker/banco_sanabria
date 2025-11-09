using BancoSanabria.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace BancoSanabria.API.Middleware
{
    /// <summary>
    /// Middleware global para manejo de excepciones
    /// </summary>
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse();

            switch (exception)
            {
                case BusinessException businessException:
                    // Excepciones de negocio controladas (400 Bad Request)
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = context.Response.StatusCode;
                    response.Mensaje = businessException.Message;
                    _logger.LogWarning(businessException, "Excepción de negocio: {Message}", businessException.Message);
                    break;

                case KeyNotFoundException:
                    // Recurso no encontrado (404 Not Found)
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusCode = context.Response.StatusCode;
                    response.Mensaje = exception.Message;
                    _logger.LogWarning(exception, "Recurso no encontrado: {Message}", exception.Message);
                    break;

                case ArgumentException argumentException:
                    // Argumentos inválidos (400 Bad Request)
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = context.Response.StatusCode;
                    response.Mensaje = argumentException.Message;
                    _logger.LogWarning(argumentException, "Argumento inválido: {Message}", argumentException.Message);
                    break;

                default:
                    // Error interno del servidor (500 Internal Server Error)
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.StatusCode = context.Response.StatusCode;
                    response.Mensaje = "Ha ocurrido un error interno en el servidor";
                    _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    /// <summary>
    /// Extension method para registrar el middleware
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}

