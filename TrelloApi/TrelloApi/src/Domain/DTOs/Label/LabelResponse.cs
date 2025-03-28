using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.CardLabel;

namespace TrelloApi.Domain.DTOs.Label;

public class LabelResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int BoardId { get; set; }
    public BoardResponse Board;
    public ICollection<CardLabelResponse> CardLabels;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}