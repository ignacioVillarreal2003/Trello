using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("List")]
public class List
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(64), Required]
    public string Title { get; set; }
    
    public int Position { get; set; }
    
    [ForeignKey("Board"), Required]
    public int BoardId { get; set; }
    public Board Board { get; set; }
    
    public ICollection<Card> Tasks { get; set; } = new HashSet<Card>();
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List(string title, int boardId, int position = 0)
    {
        Title = title;
        BoardId = boardId;
        Position = position;
    }
}
