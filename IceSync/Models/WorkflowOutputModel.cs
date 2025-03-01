namespace IceSync.Models;

public class WorkflowOutputModel
{
    public long WorkflowId { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? MultiExecBehaviour { get; set; }
}
