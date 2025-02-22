namespace TrelloApi.Domain.DTOs.Card;

public class CardResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ListId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}