using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputListDetailsDto
{
    public int Id { get; set; }
        
    public string Title { get; set; } = string.Empty;
        
    public int Position { get; set; }
        
    public int BoardId { get; set; }
        
    public DateTime CreatedAt { get; set; }
        
    public DateTime? UpdatedAt { get; set; }
}
    
public class AddListDto
{
    [Required, StringLength(32)]
    public string Title { get; set; } = string.Empty;
        
    [Required]
    public int Position { get; set; }
}
    
public class UpdateListDto
{
    [StringLength(32)]
    public string? Title { get; set; }
        
    public int? Position { get; set; }
}