namespace TrelloApi.Application.Utils;

public interface IEncrypt
{
    string HashPassword(string password);
    bool ComparePassword(string inputPassword, string storedPassword);
}