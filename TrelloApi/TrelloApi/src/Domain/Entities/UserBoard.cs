using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.Entities;
    
[Table("UserBoard")]
public class UserBoard
{
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("Board")]
    public int BoardId { get; set; }
    public Board Board { get; set; }
    
    [StringLength(32)]
    public string Role { get; set; }

    public UserBoard(int userId, int boardId, string role = "Member")
    {
        UserId = userId;
        BoardId = boardId;
        Role = role;
    }
}