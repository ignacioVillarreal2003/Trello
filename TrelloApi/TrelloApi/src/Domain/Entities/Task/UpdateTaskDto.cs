using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Task.DTO;

public class UpdateTaskDto
{
    [StringLength(64)] 
    public string? Title { get; set; }
    
    [StringLength(256)] 
    public string? Description { get; set; }
    
    public int? ListId { get; set; }
    
    [StringLength(32)]
    public string? Priority { get; set; }
    
    public bool? IsCompleted { get; set; }
}