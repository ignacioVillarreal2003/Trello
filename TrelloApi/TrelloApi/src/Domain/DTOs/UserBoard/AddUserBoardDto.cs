namespace TrelloApi.Domain.DTOs.UserBoard;

public class AddUserBoardDto
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}