using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.User;

namespace TrelloApi.Domain.DTOs.UserBoard;

public class UserBoardResponse
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserResponse User { get; set; }
    public BoardResponse Board { get; set; }
}