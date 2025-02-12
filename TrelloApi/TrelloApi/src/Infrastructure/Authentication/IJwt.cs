namespace TrelloApi.Infrastructure.Authentication;

public interface IJwt
{ 
    string GenerateToken(int userId);
}