using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.List;

public class UpdateListDto
{
    [StringLength(64)] 
    public string? Title { get; set; }
    
    public int? Position { get; set; }

}