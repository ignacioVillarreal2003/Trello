using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.User;

namespace TrelloApi.Domain.DTOs.UserBoard;

public class UserBoardResponse
{
    public int BoardId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public UserResponse User { get; set; }
    public BoardResponse Board { get; set; }
}