using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Board")]
public class Board
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(32), Required]
    public string Title { get; set; }
    
    [StringLength(256)] 
    public string Description { get; set; }

    [StringLength(8), Required]
    public string Color { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
    
    public bool IsArchived { get; set; } = false;

    [DataType(DataType.DateTime)]
    public DateTime? ArchivedAt { get; set; } = null;
    
    public ICollection<List> Lists { get; set; } = new HashSet<List>();

    public ICollection<UserBoard> UserBoards { get; set; } = new HashSet<UserBoard>();

    public ICollection<Label> Labels { get; set; } = new HashSet<Label>();

    public Board(string title, string color, string description = "")
    {
        Title = title;
        Color = color;
        Description = description;
    }
}