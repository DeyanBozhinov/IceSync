using IceSync.Services.Models;

namespace IceSync.Services;

public interface IWorkflowService
{
    Task<OutputTypeModel<List<WorkflowDTO>>> GetAllAsync();
    Task<OutputTypeModel<string?>> RunAsync(int model);
    Task WorkflowTableSync(CancellationToken token);
}
