using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.Label;

public class AddLabelDto
{
    [StringLength(64), Required] 
    public string Title { get; set; }

    [StringLength(8), Required] 
    public string Color { get; set; }
}