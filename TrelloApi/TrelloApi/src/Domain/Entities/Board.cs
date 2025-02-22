using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Board")]
public class Board: Entity
{
    [StringLength(32), Required]
    public string Title { get; set; }
    
    [StringLength(256)]
    public string? Description { get; set; }

    [StringLength(32), Required]
    public string Background { get; set; }
    public bool IsArchived { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? ArchivedAt { get; set; }
    
    public ICollection<List> Lists { get; set; }

    public ICollection<UserBoard> UserBoards { get; set; }

    public ICollection<Label> Labels { get; set; }

    public Board(string title, string background, string? description = null)
    {
        Title = title;
        Description = description;
        Background = background;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
        IsArchived = false;
        ArchivedAt = null;
    }
}