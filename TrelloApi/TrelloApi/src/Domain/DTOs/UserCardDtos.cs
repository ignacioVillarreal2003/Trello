using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputUserCardDto
{
    public int UserId { get; set; }
        
    public int CardId { get; set; }
}
    
public class AddUserCardDto
{
    [Required]
    public int UserId { get; set; }
        
    [Required]
    public int CardId { get; set; }
}