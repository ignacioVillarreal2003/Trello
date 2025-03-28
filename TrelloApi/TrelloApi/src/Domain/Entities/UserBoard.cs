using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;
    
[Table("UserBoard")]
public class UserBoard: Entity
{
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("Board")]
    public int BoardId { get; set; }
    public Board Board { get; set; }

    public UserBoard(int userId, int boardId)
    {
        UserId = userId;
        BoardId = boardId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }
}