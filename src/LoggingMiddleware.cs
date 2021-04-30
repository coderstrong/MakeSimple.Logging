using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MakeSimple.Logging
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            LogContext.PushProperty("Method", context.Request.Method);
            LogContext.PushProperty("QueryString", context.Request.QueryString.ToString());
            // check if the Request is a POST|PUT call 
            // since we need to read from the body
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body)
                                                    .ReadToEndAsync();
                context.Request.Body.Position = 0;
                LogContext.PushProperty("Payload", body);
            }
            LogContext.PushProperty("RequestedOn", DateTime.Now);

            await _next.Invoke(context);

            using Stream originalRequest = context.Response.Body;
            try
            {
                using var memStream = new MemoryStream();
                context.Response.Body = memStream;
                // All the Request processing as described above 
                // happens from here.
                // Response handling starts from here
                // set the pointer to the beginning of the 
                // memory stream to read
                memStream.Position = 0;
                // read the memory stream till the end
                var response = await new StreamReader(memStream)
                                                        .ReadToEndAsync();
                // write the response to the log object
                LogContext.PushProperty("Response", response);
                LogContext.PushProperty("ResponseCode", context.Response.StatusCode);
                LogContext.PushProperty("RespondedOn", DateTime.Now);

                // since we have read till the end of the stream, 
                // reset it onto the first position
                memStream.Position = 0;

                // now copy the content of the temporary memory 
                // stream we have passed to the actual response body 
                // which will carry the response out.
                await memStream.CopyToAsync(originalRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Response stream ex!");
            }
            finally
            {
                // assign the response body to the actual context
                context.Response.Body = originalRequest;
            }
        }
    }
}
