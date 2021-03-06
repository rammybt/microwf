using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  [Authorize]
  [Route("api/workflow")]
  public class WorkflowController : Controller
  {
    private readonly IWorkflowService service;
    private readonly IJobQueueService jobQueueService;

    public WorkflowController(
      IWorkflowService service,
      IJobQueueService jobQueueService
    )
    {
      this.service = service;
      this.jobQueueService = jobQueueService;
    }

    [HttpGet()]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<WorkflowViewModel>), 200)]
    public async Task<ActionResult<PaginatedList<WorkflowViewModel>>> GetWorkflows(
      [FromQuery] WorkflowSearchPagingParameters pagingParameters
    )
    {
      PaginatedList<WorkflowViewModel> result
        = await this.service.GetWorkflowsAsync(pagingParameters);

      this.AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(WorkflowViewModel), 200)]
    public async Task<ActionResult<WorkflowViewModel>> Get(int id)
    {
      var result = await this.service.GetAsync(id);

      return Ok(result);
    }

    [HttpGet("{id}/history")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkflowHistoryViewModel>), 200)]
    public async Task<ActionResult<IEnumerable<WorkflowHistoryViewModel>>> GetHistory(int id)
    {
      var result = await this.service.GetHistoryAsync(id);

      return Ok(result);
    }

    [HttpGet("{id}/variables")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkflowVariableViewModel>), 200)]
    public async Task<ActionResult<IEnumerable<WorkflowVariableViewModel>>> GetVariables(int id)
    {
      var result = await this.service.GetVariablesAsync(id);

      return Ok(result);
    }

    [HttpGet("definitions")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowDefinitionViewModel>), 200)]
    public ActionResult<IEnumerable<WorkflowDefinitionViewModel>> GetWorkflowDefinitions()
    {
      var result = this.service.GetWorkflowDefinitions();

      return Ok(result);
    }

    [HttpGet("dot/{type}")]
    [ProducesResponseType(typeof(string), 200)]
    public ActionResult<string> Dot(string type)
    {
      var result = this.service.Dot(type);

      return Ok(result);
    }

    [HttpGet("dotwithhistory/{type}/{correlationId}")]
    [ProducesResponseType(typeof(WorkflowViewModel), 200)]
    public async Task<ActionResult<WorkflowViewModel>> DotWithHistory(string type, int correlationId)
    {
      var result = await this.service.DotWithHistoryAsync(type, correlationId);

      return Ok(result);
    }

    private void AddXPagination(
      PagingParameters pagingParameters,
      PaginatedList<WorkflowViewModel> result
    )
    {
      var paginationMetadata = new
      {
        totalCount = result.AllItemsCount,
        pageSize = pagingParameters.PageSize,
        pageIndex = pagingParameters.PageIndex,
        totalPages = result.TotalPages
      };

      this.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
    }
  }
}
