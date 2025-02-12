using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.User;

public class RegisterUserDto
{
    [StringLength(64), EmailAddress, Required] 
    public string Email { get; set; }
    
    [StringLength(64), Required] 
    public string Username { get; set; }
    
    [StringLength(64), Required] 
    public string Password { get; set; }
}