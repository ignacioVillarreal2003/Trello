using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Comment.DTO;

public class UpdateCommentDto
{
    [StringLength(256), Required] 
    public string Text { get; set; }
}