using System.Diagnostics;

namespace Portfolio.Api.Middlewares;

public class ResponseTimeMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context)
	{
		var start = Stopwatch.GetTimestamp();

		context.Response.OnStarting(() =>
		{
			var elapse = Stopwatch.GetElapsedTime(start);
			context.Response.Headers["X-Response-Time"] = $"{elapse.TotalMilliseconds:F2}ms";
			return Task.CompletedTask;
		});

		await next(context);

	}
}