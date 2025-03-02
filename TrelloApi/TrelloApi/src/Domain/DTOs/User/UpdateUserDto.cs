namespace TrelloApi.Domain.DTOs.User;

public class UpdateUserDto
{
    public string? Username { get; set; }
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? Theme { get; set; }
}