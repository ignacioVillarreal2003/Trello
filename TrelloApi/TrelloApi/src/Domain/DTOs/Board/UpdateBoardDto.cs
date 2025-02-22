namespace TrelloApi.Domain.DTOs.Board;

public class UpdateBoardDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Background { get; set; }
    public bool? IsArchived { get; set; }
}
