using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrelloApi.Domain.Entities.List;

namespace TrelloApi.Domain.Board;

[Table("Board")]
public class Board
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(64), Required]
    public string Title { get; set; }
    
    [StringLength(32), Required]
    public string Theme { get; set; }
    
    [StringLength(64), Required]
    public string Icon { get; set; }

    [StringLength(256)] public string Description { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsArchived { get; set; } = false;

    public ICollection<List> Lists { get; set; } = new HashSet<List>();

    public ICollection<UserBoard.UserBoard> UserBoards { get; set; } = new HashSet<UserBoard.UserBoard>();

    public ICollection<Label.Label> Labels { get; set; } = new HashSet<Label.Label>();

    public Board(string title, string icon, string theme = "Blue", string description = "")
    {
        Title = title;
        Theme = theme;
        Icon = icon;
        Description = description;
    }
}