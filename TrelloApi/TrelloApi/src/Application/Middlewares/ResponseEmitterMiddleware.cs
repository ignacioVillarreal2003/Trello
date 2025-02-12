namespace TrelloApi.Application.Middlewares;

public class ResponseEmitterMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseEmitterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string origin = context.Request.Headers["Origin"].ToString() ?? string.Empty;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
            context.Response.Headers["Access-Control-Allow-Origin"] = origin;
            context.Response.Headers["Access-Control-Allow-Headers"] =
                "X-Requested-With, Content-Type, Accept, Origin, Authorization";
            context.Response.Headers["Access-Control-Allow-Methods"] =
                "GET, POST, PUT, PATCH, DELETE, OPTIONS";
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            context.Response.Headers.Append("Cache-Control", "post-check=0, pre-check=0");
            context.Response.Headers["Pragma"] = "no-cache";

            return Task.CompletedTask;
        });

        await _next(context);
    }
}