using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputLabelDetailsDto
{
    public int Id { get; set; }
        
    public string Title { get; set; } = string.Empty;
        
    public string Color { get; set; } = string.Empty;
        
    public int BoardId { get; set; }
}
    
public class AddLabelDto
{
    [Required, StringLength(32)]
    public string Title { get; set; } = string.Empty;
        
    [Required, StringLength(8)]
    public string Color { get; set; } = string.Empty;
        
    [Required]
    public int BoardId { get; set; }
}
    
public class UpdateLabelDto
{
    [StringLength(32)]
    public string? Title { get; set; }
        
    [StringLength(8)]
    public string? Color { get; set; }
}