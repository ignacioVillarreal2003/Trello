using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Label")]
public class Label: Entity
{
    [StringLength(32), Required] 
    public string Title { get; set; }

    [StringLength(8), Required] 
    public string Color { get; set; }

    [ForeignKey("Board"), Required]
    public int BoardId { get; set; }
    public Board Board { get; set; }

    public ICollection<CardLabel> CardLabels { get; set; }

    public Label(string title, string color, int boardId)
    {
        Title = title;
        Color = color;
        BoardId = boardId;
    }
}