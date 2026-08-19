using System.Net;
using System.Text.Json;
using DevJourney.Application.Exceptions;

namespace DevJourney.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var code = HttpStatusCode.InternalServerError;
            var error = new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "server_error",
                    Message = "An unexpected error occurred.",
                    Details = new Dictionary<string, string[]>()
                }
            };

            switch (exception)
            {
                case NotFoundException nf:
                    code = HttpStatusCode.NotFound;
                    error.Error.Code = "not_found";
                    error.Error.Message = nf.Message;
                    break;
                case ConflictException cf:
                    code = HttpStatusCode.Conflict;
                    error.Error.Code = "conflict";
                    error.Error.Message = cf.Message;
                    break;
                case ValidationException ve:
                    code = (HttpStatusCode)422;
                    error.Error.Code = "validation_error";
                    error.Error.Message = "Validation failed.";

                    // Attempt to parse field from message if formatted as "Validation failed for 'field': message"
                    var details = new Dictionary<string, string[]>();
                    var msg = ve.Message;
                    const string pattern = "Validation failed for '";
                    if (msg.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var after = msg.Substring(pattern.Length);
                            var idx = after.IndexOf('\'');
                            var field = after.Substring(0, idx);
                            var restStart = after.IndexOf(": ", idx) + 2;
                            var rest = restStart > 1 ? after.Substring(restStart) : msg;
                            details[field] = new[] { rest };
                        }
                        catch
                        {
                            details["error"] = new[] { msg };
                        }
                    }
                    else
                    {
                        details["error"] = new[] { msg };
                    }

                    error.Error.Details = details.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                default:
                    _logger.LogError(exception, "Unhandled exception");
                    break;
            }

            context.Response.StatusCode = (int)code;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(error, options);
            return context.Response.WriteAsync(payload);
        }

        private class ErrorResponse
        {
            public ErrorDetail Error { get; set; } = new ErrorDetail();
        }

        private class ErrorDetail
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public Dictionary<string, string[]> Details { get; set; } = new Dictionary<string, string[]>();
        }
    }
}
