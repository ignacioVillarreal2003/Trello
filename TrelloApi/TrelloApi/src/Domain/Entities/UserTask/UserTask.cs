using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities.UserTask;

[Table("UserTask")]
public class UserTask
{
    [ForeignKey("User")]
    public int UserId { get; set; }
    public Entities.User.User User { get; set; }
    
    [ForeignKey("Task")]
    public int TaskId { get; set; }
    public Domain.Task.Task Task { get; set; }

    public UserTask(int userId, int taskId)
    {
        UserId = userId;
        TaskId = taskId;
    }
}