using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Comment;

[Table("Comment")]
public class Comment
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(256), Required]
    public string Text { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("Task"), Required]
    public int TaskId { get; set; }
    public Task.Task Task { get; set; }
    
    [ForeignKey("User"), Required]
    public int AuthorId { get; set; }
    public Entities.User.User User { get; set; }

    public Comment(string text, int taskId, int authorId)
    {
        Text = text;
        TaskId = taskId;
        AuthorId = authorId;
    }
}