using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrelloApi.Domain.Entities.TaskLabel;

namespace TrelloApi.Domain.Label;

[Table("Label")]
public class Label
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }

    [StringLength(64), Required] 
    public string Title { get; set; }

    [StringLength(8), Required] 
    public string Color { get; set; }

    [ForeignKey("Board"), Required]
    public int BoardId { get; set; }
    public Board.Board Board { get; set; }

    public ICollection<TaskLabel> TaskLabels { get; set; } = new HashSet<TaskLabel>();

    public Label(string title, string color, int boardId)
    {
        Title = title;
        Color = color;
        BoardId = boardId;
    }
}