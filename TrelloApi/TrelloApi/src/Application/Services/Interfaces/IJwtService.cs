namespace TrelloApi.Application.Services.Interfaces;

public interface IJwtService
{ 
    string GenerateAccessToken(int userId);
    string GenerateRefreshToken();
    Task SaveRefreshToken(int userId, string refreshToken);
    Task<int?> ValidateRefreshToken(string refreshToken);
}