using System.ComponentModel.DataAnnotations;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs;

public class OutputUserDetailsDto
{
    public int Id { get; set; }
    
    public string Email { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    
    public string Theme { get; set; } = string.Empty;
}

public class LoginUserDto
{
    [StringLength(64), EmailAddress, Required] 
    public string Email { get; set; } = string.Empty;

    [StringLength(64), Required] 
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserDto
{
    [StringLength(64), EmailAddress, Required] 
    public string Email { get; set; } = string.Empty;
    
    [StringLength(64), Required] 
    public string Username { get; set; } = string.Empty;
    
    [StringLength(64), Required] 
    public string Password { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    [StringLength(64)] 
    public string? Username { get; set; } = string.Empty;
    
    [StringLength(64)] 
    public string? OldPassword { get; set; } = string.Empty;
    
    [StringLength(64)] 
    public string? NewPassword { get; set; } = string.Empty;
    
    [StringLength(32)] 
    public string? Theme { get; set; } = string.Empty;
}