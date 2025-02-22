namespace TrelloApi.Domain.DTOs.Card;

public class AddCardDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Priority { get; set; }
}


