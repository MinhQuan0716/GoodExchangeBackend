using System.Diagnostics;

namespace MobileAPI.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        public PerformanceMiddleware(RequestDelegate next)
        {
            _next=next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            context.Response.OnStarting(() =>
            {
                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Request [{context.Request.Method}] at [{context.Request.Path}] took {responseTime} ms");
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
