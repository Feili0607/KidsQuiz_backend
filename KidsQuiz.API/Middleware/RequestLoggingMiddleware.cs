using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KidsQuiz.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = await FormatRequest(context.Request);
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                    stopwatch.Stop();

                    var response = await FormatResponse(context.Response);
                    _logger.LogInformation(
                        "Request: {Method} {Path} {Query} {RequestBody}\n" +
                        "Response: {StatusCode} {ResponseBody}\n" +
                        "Duration: {Duration}ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Request.QueryString,
                        request,
                        context.Response.StatusCode,
                        response,
                        stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the request");
                    throw;
                }
                finally
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    context.Response.Body = originalBodyStream;
                }
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            var sb = new StringBuilder();
            sb.AppendLine($"Method: {request.Method}");
            sb.AppendLine($"Path: {request.Path}");
            sb.AppendLine($"QueryString: {request.QueryString}");
            sb.AppendLine($"Headers: {FormatHeaders(request.Headers)}");
            sb.AppendLine($"Body: {body}");

            return sb.ToString();
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            var sb = new StringBuilder();
            sb.AppendLine($"StatusCode: {response.StatusCode}");
            sb.AppendLine($"Headers: {FormatHeaders(response.Headers)}");
            sb.AppendLine($"Body: {body}");

            return sb.ToString();
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var sb = new StringBuilder();
            foreach (var header in headers)
            {
                sb.AppendLine($"{header.Key}: {header.Value}");
            }
            return sb.ToString();
        }
    }
} 