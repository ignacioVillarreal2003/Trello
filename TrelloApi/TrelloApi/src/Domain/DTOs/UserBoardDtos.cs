using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputUserBoardDto
{
    public int Id { get; set; }
        
    public string Email { get; set; } = string.Empty;
        
    public string Username { get; set; } = string.Empty;
        
    public string Theme { get; set; } = string.Empty;
        
    public DateTime CreatedAt { get; set; }
        
    public DateTime LastLogin { get; set; }
}

public class AddUserBoardDto
{
    [Required]
    public int UserId { get; set; }
        
    [StringLength(32)]
    public string? Role { get; set; }
}

public class OutputUserBoardListDto
{
    public int BoardId { get; set; }
    public int UserId { get; set; }
        
    public string Role { get; set; } = "Member";
}