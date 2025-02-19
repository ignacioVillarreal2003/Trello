using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputUserCardDetailsDto
{
    public int UserId { get; set; }
        
    public int CardId { get; set; }
}
    
public class AddUserCardDto
{
    [Required]
    public int UserId { get; set; }
}