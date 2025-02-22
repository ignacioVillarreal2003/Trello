namespace TrelloApi.Domain.DTOs.Card;

public class UpdateCardDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? ListId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public bool? IsCompleted { get; set; }
}