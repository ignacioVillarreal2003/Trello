using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputUserBoardDetailsDto
{
    public int BoardId { get; set; }
    public int UserId { get; set; }
        
    public string Role { get; set; } = string.Empty;
}

public class AddUserBoardDto
{
    [Required]
    public int UserId { get; set; }

    [Required, StringLength(32)] 
    public string Role { get; set; } = string.Empty;
}

