namespace TrelloApi.Domain.Entities.Board;

public class OutputBoardDto
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Theme { get; set; }
    
    public string Icon { get; set; }
    
    public string Description { get; set; }
    
    public bool IsArchived { get; set; }
}