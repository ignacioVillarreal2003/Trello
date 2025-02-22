namespace TrelloApi.Domain.DTOs.UserBoard;

public class UserBoardResponse
{
    public int BoardId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}