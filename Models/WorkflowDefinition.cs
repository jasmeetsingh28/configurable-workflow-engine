namespace WorkflowEngine.Models
{
    public class WorkflowDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<State> States { get; set; } = new();
        public List<WorkflowAction> Actions { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
