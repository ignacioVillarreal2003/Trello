using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.Label;

namespace TrelloApi.Domain.DTOs.CardLabel;

public class CardLabelResponse
{
    public int CardId { get; set; }
    public int LabelId { get; set; }
    public CardResponse Card { get; set; }
    public LabelResponse Label { get; set; }
}