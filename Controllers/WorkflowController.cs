using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Models;
using WorkflowEngine.Models.DTOs;
using WorkflowEngine.Services;

namespace WorkflowEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;

        public WorkflowController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        /// <summary>
        /// Creates a new workflow definition with states and actions
        /// </summary>
        [HttpPost("definitions")]
        public async Task<ActionResult<WorkflowDefinition>> CreateDefinition([FromBody] CreateWorkflowDefinitionRequest request)
        {
            try
            {
                var definition = await _workflowService.CreateDefinitionAsync(request);
                return Ok(definition);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all workflow definitions
        /// </summary>
        [HttpGet("definitions")]
        public async Task<ActionResult<List<WorkflowDefinition>>> GetDefinitions()
        {
            var definitions = await _workflowService.GetAllDefinitionsAsync();
            return Ok(definitions);
        }

        /// <summary>
        /// Retrieves a specific workflow definition by ID
        /// </summary>
        [HttpGet("definitions/{id}")]
        public async Task<ActionResult<WorkflowDefinition>> GetDefinition(string id)
        {
            var definition = await _workflowService.GetDefinitionAsync(id);
            if (definition == null)
            {
                return NotFound(new { error = "Definition not found" });
            }
            return Ok(definition);
        }

        /// <summary>
        /// Starts a new workflow instance from a definition
        /// </summary>
        [HttpPost("definitions/{definitionId}/instances")]
        public async Task<ActionResult<WorkflowInstance>> StartInstance(string definitionId)
        {
            try
            {
                var instance = await _workflowService.StartInstanceAsync(definitionId);
                return Ok(instance);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all workflow instances
        /// </summary>
        [HttpGet("instances")]
        public async Task<ActionResult<List<WorkflowInstance>>> GetInstances()
        {
            var instances = await _workflowService.GetAllInstancesAsync();
            return Ok(instances);
        }

        /// <summary>
        /// Retrieves a specific workflow instance by ID
        /// </summary>
        [HttpGet("instances/{id}")]
        public async Task<ActionResult<WorkflowInstance>> GetInstance(string id)
        {
            var instance = await _workflowService.GetInstanceAsync(id);
            if (instance == null)
            {
                return NotFound(new { error = "Instance not found" });
            }
            return Ok(instance);
        }

        /// <summary>
        /// Executes an action on a workflow instance to transition states
        /// </summary>
        [HttpPost("instances/{instanceId}/execute")]
        public async Task<ActionResult<WorkflowInstance>> ExecuteAction(string instanceId, [FromBody] ExecuteActionRequest request)
        {
            try
            {
                var instance = await _workflowService.ExecuteActionAsync(instanceId, request);
                return Ok(instance);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
