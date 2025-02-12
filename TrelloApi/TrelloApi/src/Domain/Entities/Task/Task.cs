using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrelloApi.Domain.Entities.List;
using TrelloApi.Domain.Entities.TaskLabel;

namespace TrelloApi.Domain.Task;

[Table("Task")]
public class Task
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(64), Required]
    public string Title { get; set; }
    
    [StringLength(256), Required]
    public string Description { get; set; }
    
    [ForeignKey("List"), Required]
    public int ListId { get; set; }
    public List List { get; set; }
    
    public ICollection<Comment.Comment> Comments { get; set; } = new HashSet<Comment.Comment>();
    
    public ICollection<TaskLabel> TaskLabels { get; set; } = new HashSet<TaskLabel>();
    
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
    
    [StringLength(32)]
    public string Priority { get; set; }
    
    public bool IsCompleted { get; set; } = false;
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Task(string title, string description, int listId, string priority = "Medium")
    {
        Title = title;
        Description = description;
        ListId = listId;
        Priority = priority;
    }
}
