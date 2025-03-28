using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("CardLabel")]
public class CardLabel: Entity
{
    [ForeignKey("Card")]
    public int CardId { get; set; }
    public Card Card { get; set; }
        
    [ForeignKey("Label")]
    public int LabelId { get; set; }
    public Label Label { get; set; }

    public CardLabel(int cardId, int labelId)
    {
        CardId = cardId;
        LabelId = labelId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }
}