using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.User;

namespace TrelloApi.Domain.DTOs.UserCard;

public class UserCardResponse
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public UserResponse User { get; set; }
    public CardResponse Card { get; set; }
}