namespace TrelloApi.Domain.DTOs.User;

public class UpdateUserDto
{
    public string? Username { get; set; } = string.Empty;
    public string? OldPassword { get; set; } = string.Empty;
    public string? NewPassword { get; set; } = string.Empty;
    public string? Theme { get; set; } = string.Empty;
}