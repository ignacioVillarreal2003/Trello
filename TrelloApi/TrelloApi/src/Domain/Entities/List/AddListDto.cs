using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.List;

public class AddListDto
{
    [StringLength(64), Required] 
    public string Title { get; set; }
    
    public int? Position { get; set; }
}