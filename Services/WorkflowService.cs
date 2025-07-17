using WorkflowEngine.Models;
using WorkflowEngine.Models.DTOs;
using System.Collections.Concurrent;

namespace WorkflowEngine.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
        private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();

        public async Task<WorkflowDefinition> CreateDefinitionAsync(CreateWorkflowDefinitionRequest request)
        {
            ValidateDefinition(request);

            var definition = new WorkflowDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                States = request.States,
                Actions = request.Actions
            };

            _definitions[definition.Id] = definition;
            return await Task.FromResult(definition);
        }

        public async Task<WorkflowDefinition?> GetDefinitionAsync(string definitionId)
        {
            _definitions.TryGetValue(definitionId, out var definition);
            return await Task.FromResult(definition);
        }

        public async Task<List<WorkflowDefinition>> GetAllDefinitionsAsync()
        {
            return await Task.FromResult(_definitions.Values.ToList());
        }

        public async Task<WorkflowInstance> StartInstanceAsync(string definitionId)
        {
            var definition = await GetDefinitionAsync(definitionId);
            if (definition == null)
            {
                throw new ArgumentException($"Definition with ID {definitionId} not found");
            }

            var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
            if (initialState == null)
            {
                throw new InvalidOperationException("No initial state found in definition");
            }

            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = definitionId,
                CurrentStateId = initialState.Id
            };

            _instances[instance.Id] = instance;
            return instance;
        }

        public async Task<WorkflowInstance> ExecuteActionAsync(string instanceId, ExecuteActionRequest request)
        {
            var instance = await GetInstanceAsync(instanceId);
            if (instance == null)
            {
                throw new ArgumentException($"Instance with ID {instanceId} not found");
            }

            var definition = await GetDefinitionAsync(instance.DefinitionId);
            if (definition == null)
            {
                throw new InvalidOperationException("Definition not found for instance");
            }

            var action = definition.Actions.FirstOrDefault(a => a.Id == request.ActionId);
            if (action == null)
            {
                throw new ArgumentException($"Action with ID {request.ActionId} not found in definition");
            }

            ValidateActionExecution(instance, action, definition);

            var fromState = definition.States.First(s => s.Id == instance.CurrentStateId);
            var toState = definition.States.First(s => s.Id == action.ToState);

            instance.CurrentStateId = action.ToState;
            instance.History.Add(new HistoryEntry
            {
                ActionId = action.Id,
                ActionName = action.Name,
                FromStateId = fromState.Id,
                ToStateId = toState.Id
            });

            return instance;
        }

        public async Task<WorkflowInstance?> GetInstanceAsync(string instanceId)
        {
            _instances.TryGetValue(instanceId, out var instance);
            return await Task.FromResult(instance);
        }

        public async Task<List<WorkflowInstance>> GetAllInstancesAsync()
        {
            return await Task.FromResult(_instances.Values.ToList());
        }

        private void ValidateDefinition(CreateWorkflowDefinitionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Definition name is required");
            }

            if (!request.States.Any())
            {
                throw new ArgumentException("At least one state is required");
            }

            var stateIds = request.States.Select(s => s.Id).ToList();
            if (stateIds.Distinct().Count() != stateIds.Count)
            {
                throw new ArgumentException("Duplicate state IDs found");
            }

            var actionIds = request.Actions.Select(a => a.Id).ToList();
            if (actionIds.Distinct().Count() != actionIds.Count)
            {
                throw new ArgumentException("Duplicate action IDs found");
            }

            var initialStates = request.States.Where(s => s.IsInitial).ToList();
            if (initialStates.Count != 1)
            {
                throw new ArgumentException("Exactly one initial state is required");
            }

            var validStateIds = new HashSet<string>(stateIds);
            foreach (var action in request.Actions)
            {
                if (!validStateIds.Contains(action.ToState))
                {
                    throw new ArgumentException($"Action {action.Id} references unknown target state {action.ToState}");
                }

                foreach (var fromState in action.FromStates)
                {
                    if (!validStateIds.Contains(fromState))
                    {
                        throw new ArgumentException($"Action {action.Id} references unknown source state {fromState}");
                    }
                }
            }
        }

        private void ValidateActionExecution(WorkflowInstance instance, WorkflowAction action, WorkflowDefinition definition)
        {
            if (!action.Enabled)
            {
                throw new InvalidOperationException($"Action {action.Id} is disabled");
            }

            if (!action.FromStates.Contains(instance.CurrentStateId))
            {
                throw new InvalidOperationException($"Action {action.Id} cannot be executed from current state {instance.CurrentStateId}");
            }

            var currentState = definition.States.First(s => s.Id == instance.CurrentStateId);
            if (currentState.IsFinal)
            {
                throw new InvalidOperationException("Cannot execute actions on final states");
            }

            var targetState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
            if (targetState == null)
            {
                throw new InvalidOperationException($"Target state {action.ToState} not found");
            }
        }
    }
}
