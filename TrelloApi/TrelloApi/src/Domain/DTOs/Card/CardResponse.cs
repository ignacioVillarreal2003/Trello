using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Comment;
using TrelloApi.Domain.DTOs.List;

namespace TrelloApi.Domain.DTOs.Card;

public class CardResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ListId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Position { get; set; }
    public ListResponse List;
    public ICollection<CommentResponse> Comments;

    public ICollection<CardLabelResponse> CardLabels;
}