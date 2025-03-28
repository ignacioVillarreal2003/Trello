using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Board")]
public class Board: Entity
{
    [StringLength(32), Required]
    public string Title { get; set; }

    [StringLength(32), Required]
    public string Background { get; set; }
    
    public ICollection<List> Lists { get; set; }

    public ICollection<UserBoard> UserBoards { get; set; }

    public ICollection<Label> Labels { get; set; }

    public Board(string title, string background)
    {
        Title = title;
        Background = background;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }
}