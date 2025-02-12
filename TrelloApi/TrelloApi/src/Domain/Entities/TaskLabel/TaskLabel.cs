using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities.TaskLabel;

[Table("TaskLabel")]
public class TaskLabel
{
    [ForeignKey("Task")]
    public int TaskId { get; set; }
    public Domain.Task.Task Task { get; set; }
        
    [ForeignKey("Label")]
    public int LabelId { get; set; }
    public Domain.Label.Label Label { get; set; }

    public TaskLabel(int taskId, int labelId)
    {
        TaskId = taskId;
        LabelId = labelId;
    }
}