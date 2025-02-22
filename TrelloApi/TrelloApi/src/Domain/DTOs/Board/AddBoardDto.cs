namespace TrelloApi.Domain.DTOs.Board;

public class AddBoardDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; set; }
    public string Background { get; set; } = string.Empty;
}