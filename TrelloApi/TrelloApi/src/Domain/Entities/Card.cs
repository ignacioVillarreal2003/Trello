using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Card")]
public class Card
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(32), Required]
    public string Title { get; set; }
    
    [StringLength(256), Required]
    public string Description { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
    
    [StringLength(16)]
    public string Priority { get; set; }
    
    public bool IsCompleted { get; set; } = false;
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
    
    [ForeignKey("List"), Required]
    public int ListId { get; set; }
    public List List { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    
    public ICollection<CardLabel> CardLabels { get; set; } = new HashSet<CardLabel>();
    
    public Card(string title, string description, int listId, string priority)
    {
        Title = title;
        Description = description;
        ListId = listId;
        Priority = priority;
    }
}