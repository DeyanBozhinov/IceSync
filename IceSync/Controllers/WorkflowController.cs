using System.Net;
using AutoMapper;
using IceSync.Models;
using IceSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace IceSync.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _workflowService;
    private readonly IMapper _mapper;

    public WorkflowController(
        IWorkflowService workflowService,
        IMapper mapper
    )
    {
        _workflowService = workflowService;
        _mapper = mapper;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _workflowService.GetAllAsync();
        if (result.IsSuccessfull) {
            return Ok(_mapper.Map<List<WorkflowOutputModel>>(result.Model));
        }
        return ResponseByStatusCode(result.HttpStatusCode);
    }

    [HttpPost("{id}/run")]
    public async Task<IActionResult> Run(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var result = await _workflowService.RunAsync(id);
        if (result.IsSuccessfull) {
            return Ok();
        }
        return ResponseByStatusCode(result.HttpStatusCode);
    }

    private IActionResult ResponseByStatusCode(HttpStatusCode httpStatusCode)
    {
        if (httpStatusCode == HttpStatusCode.Unauthorized) {
            return Unauthorized();
        }
        return BadRequest();
    }
}
