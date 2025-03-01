using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IceSync.DataCore.Models;

[Table("Workflows")]
public class Workflow
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int WorkflowId { get; set; }
    [Column("WorkFlowName")]
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string MultiExecBehaviour { get; set; }
}

