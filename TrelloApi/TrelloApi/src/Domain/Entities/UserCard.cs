using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("UserCard")]
public class UserCard
{
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("Card")]
    public int CardId { get; set; }
    public Card Card { get; set; }

    public UserCard(int userId, int cardId)
    {
        UserId = userId;
        CardId = cardId;
    }
}