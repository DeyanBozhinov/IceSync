using System.Text.Json.Serialization;

namespace IceSync.Services.Models;

public class WorkflowDTO
{
    [JsonPropertyName("id")]
    public int WorkflowId { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    [JsonPropertyName("multiExecBehavior")]
    public string? MultiExecBehaviour { get; set; }
}
