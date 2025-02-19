using System.ComponentModel.DataAnnotations;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs;

public class OutputCardDetailsDto
{
    public int Id { get; set; }
        
    public string Title { get; set; } = string.Empty;
        
    public string Description { get; set; } = string.Empty;
        
    public int ListId { get; set; }
        
    public DateTime? DueDate { get; set; }
        
    public string? Priority { get; set; }
        
    public bool IsCompleted { get; set; }
        
    public DateTime CreatedAt { get; set; }
        
    public DateTime? UpdatedAt { get; set; }
}

public class AddCardDto
{
    [Required, StringLength(32)]
    public string Title { get; set; } = string.Empty;
        
    [Required, StringLength(256)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(32)]
    public string? Priority { get; set; }
}

public class UpdateCardDto
{
    [StringLength(32)]
    public string? Title { get; set; }
        
    [StringLength(256)]
    public string? Description { get; set; }
    
    public int? ListId { get; set; }
        
    public DateTime? DueDate { get; set; }
        
    [StringLength(32)]
    public string? Priority { get; set; }
        
    public bool? IsCompleted { get; set; }
}