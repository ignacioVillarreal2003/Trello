using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Label")]
public class Label
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }

    [StringLength(32), Required] 
    public string Title { get; set; }

    [StringLength(8), Required] 
    public string Color { get; set; }

    [ForeignKey("Board"), Required]
    public int BoardId { get; set; }
    public Board Board { get; set; }

    public ICollection<CardLabel> TaskLabels { get; set; } = new HashSet<CardLabel>();

    public Label(string title, string color, int boardId)
    {
        Title = title;
        Color = color;
        BoardId = boardId;
    }
}