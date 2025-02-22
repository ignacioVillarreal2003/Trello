using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Card")]
public class Card: Entity
{
    [StringLength(32), Required]
    public string Title { get; set; }
    
    [StringLength(256), Required]
    public string Description { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
    
    [StringLength(16)]
    public string? Priority { get; set; }
    
    public bool IsCompleted { get; set; }
    
    [ForeignKey("List"), Required]
    public int ListId { get; set; }
    public List List { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
    
    public ICollection<CardLabel> CardLabels { get; set; }
    
    public Card(string title, string description, int listId, string? priority = null)
    {
        Title = title;
        Description = description;
        ListId = listId;
        Priority = priority;
        DueDate = null;
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }
}