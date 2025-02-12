using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TrelloApi.Application.Middlewares;

public class SessionMiddleware
{
    private readonly string _secret;
    private readonly RequestDelegate _next;

    public SessionMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _secret = configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(configuration), "JWT secret key is missing.");
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token) && ExtractUserIdFromToken(token) is int userId)
        {
            context.Items["UserId"] = userId;
        }

        await _next(context);
    }

    private int? ExtractUserIdFromToken(string token)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return int.TryParse(jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value, out var userId) ? userId : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to extract user ID from token: {ex.Message}");
            return null;
        }
    }
}
