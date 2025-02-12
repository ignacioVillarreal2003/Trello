using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.User.DTO;

public class LoginUserDto
{
    [StringLength(64), EmailAddress, Required] 
    public string Email { get; set; }

    [StringLength(64), Required] 
    public string Password { get; set; }
}