using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Label.DTO;

public class UpdateLabelDto
{
    [StringLength(64)] 
    public string? Title { get; set; }

    [StringLength(8)] 
    public string? Color { get; set; }
}