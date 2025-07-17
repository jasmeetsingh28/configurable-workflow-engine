namespace WorkflowEngine.Models.DTOs
{
    public class CreateWorkflowDefinitionRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<State> States { get; set; } = new();
        public List<WorkflowAction> Actions { get; set; } = new();
    }
}
