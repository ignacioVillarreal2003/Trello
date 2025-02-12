using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.Task;

public class AddTaskDto
{
    [StringLength(64), Required] 
    public string Title { get; set; }
    
    [StringLength(256), Required] 
    public string Description { get; set; }
    
    [StringLength(32)]
    public string? Priority { get; set; }
}