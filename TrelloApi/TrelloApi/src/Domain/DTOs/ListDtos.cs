using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputListDto
{
    public int Id { get; set; }
        
    public string Title { get; set; } = string.Empty;
        
    public int Position { get; set; }
        
    public int BoardId { get; set; }
        
    public DateTime CreatedAt { get; set; }
        
    public DateTime UpdatedAt { get; set; }
}
    
public class AddListDto
{
    [Required]
    [StringLength(64)]
    public string Title { get; set; } = string.Empty;
        
    [Required]
    public int BoardId { get; set; }
        
    public int Position { get; set; } = 0;
}
    
public class UpdateListDto
{
    [StringLength(64)]
    public string? Title { get; set; }
        
    public int? Position { get; set; }
}