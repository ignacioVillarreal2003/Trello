using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("Comment")]
public class Comment: Entity
{
    [StringLength(256), Required]
    public string Text { get; set; }
    
    [ForeignKey("Card"), Required]
    public int CardId { get; set; }
    public Card Card { get; set; }
    
    [ForeignKey("User"), Required]
    public int AuthorId { get; set; }
    public User User { get; set; }

    public Comment(string text, int cardId, int authorId)
    {
        Text = text;
        CardId = cardId;
        AuthorId = authorId;
    }
}