using TrelloApi.Domain.DTOs.Comment;
using TrelloApi.Domain.DTOs.UserBoard;
using TrelloApi.Domain.DTOs.UserCard;

namespace TrelloApi.Domain.DTOs.User;

public class UserResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public string AvatarBackground { get; set; } = string.Empty;
    public ICollection<UserBoardResponse> UserBoards;
    public ICollection<UserCardResponse> UserCards;
    public ICollection<CommentResponse> Comments;
}