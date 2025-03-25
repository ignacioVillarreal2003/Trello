namespace TrelloApi.Application.Middlewares;

public class BoardCookieMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BoardCookieMiddleware> _logger;

    public BoardCookieMiddleware(RequestDelegate next, ILogger<BoardCookieMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("BoardId", out string? boardId))
        {
            context.Items["BoardId"] = boardId;
        }

        await _next(context);
    }
}