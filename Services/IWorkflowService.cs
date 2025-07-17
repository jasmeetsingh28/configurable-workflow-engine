using WorkflowEngine.Models;
using WorkflowEngine.Models.DTOs;

namespace WorkflowEngine.Services
{
    public interface IWorkflowService
    {
        Task<WorkflowDefinition> CreateDefinitionAsync(CreateWorkflowDefinitionRequest request);
        Task<WorkflowDefinition?> GetDefinitionAsync(string definitionId);
        Task<List<WorkflowDefinition>> GetAllDefinitionsAsync();
        Task<WorkflowInstance> StartInstanceAsync(string definitionId);
        Task<WorkflowInstance> ExecuteActionAsync(string instanceId, ExecuteActionRequest request);
        Task<WorkflowInstance?> GetInstanceAsync(string instanceId);
        Task<List<WorkflowInstance>> GetAllInstancesAsync();
    }
}
