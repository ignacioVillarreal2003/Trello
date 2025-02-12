namespace TrelloApi.Application.Utils;

public static class HttpContextExtensions
{
    public static int? GetUserId(this HttpContext context)
    {
        return context.Items["UserId"] as int?;
    }
}