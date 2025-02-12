using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.User;

public class UpdateUserDto
{
    [StringLength(64)] 
    public string? Username { get; set; }
    
    [StringLength(64)] 
    public string? OldPassword { get; set; }
    
    [StringLength(64)] 
    public string? NewPassword { get; set; }
    
    [StringLength(8)] 
    public string? Theme { get; set; }
}