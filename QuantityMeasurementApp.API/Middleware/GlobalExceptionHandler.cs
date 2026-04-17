using System.Net;
using System.Text.Json;
using QuantityMeasurementApp.Business.Exceptions;
using QuantityMeasurementApp.Entity.DTOs;

namespace QuantityMeasurementApp.API.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (QuantityMeasurementException ex)
            {
                await HandleException(context, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (ArgumentException ex)
            {
                await HandleException(context, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private static async Task HandleException(
            HttpContext context, string message, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;

            ErrorResponse error = new ErrorResponse
            {
                Timestamp = DateTime.UtcNow,
                Status    = (int)statusCode,
                Error     = statusCode.ToString(),
                Message   = message,
                Path      = context.Request.Path
            };

            string json = JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(json);
        }
    }
}