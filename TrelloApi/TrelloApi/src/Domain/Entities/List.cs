using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("List")]
public class List: Entity
{
    [StringLength(32), Required]
    public string Title { get; set; }
    
    public int Position { get; set; }
    
    [ForeignKey("Board"), Required]
    public int BoardId { get; set; }
    public Board Board { get; set; }
    
    public ICollection<Card> Cards { get; set; }
    
    public List(string title, int boardId, int position)
    {
        Title = title;
        BoardId = boardId;
        Position = position;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }
}
