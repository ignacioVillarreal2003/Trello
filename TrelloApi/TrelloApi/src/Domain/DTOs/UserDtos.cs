using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class LoginUserDto
{
    [StringLength(64), EmailAddress, Required] 
    public string Email { get; set; }

    [StringLength(64), Required] 
    public string Password { get; set; }
}

public class OutputUserDto
{
    public int Id { get; set; }
    
    public string Email { get; set; }
    
    public string Username { get; set; }
    
    public string Theme { get; set; }
}

public class RegisterUserDto
{
    [StringLength(64), EmailAddress, Required] 
    public string Email { get; set; }
    
    [StringLength(64), Required] 
    public string Username { get; set; }
    
    [StringLength(64), Required] 
    public string Password { get; set; }
}

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