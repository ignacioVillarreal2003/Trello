namespace TrelloApi.Domain.Entities.Task;

public class OutputTaskDto
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }

    public string Priority { get; set; }
    
    public bool IsCompleted { get; set; }
}