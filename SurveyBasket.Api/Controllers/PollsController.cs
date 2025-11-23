namespace SurveyBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;

    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetCurrentAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CreatePollRequest request, CancellationToken cancellationToken)
    {
        var result= await _pollService.AddAsync(request, cancellationToken);
        return result.IsSuccess?Created(): result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePollRequest request, CancellationToken cancellationToken)
    {
        var updated = await _pollService.UpdateAsync(id, request, cancellationToken);
       return updated.IsSuccess ? NoContent() : updated.ToProblem();
    }
    [HttpPut("{id}/ToggelPublish")]
    public async Task<IActionResult> TogglePublishStatus(int id, CancellationToken cancellationToken)
    {
        var toggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        return toggled.IsSuccess ? NoContent() : toggled.ToProblem();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _pollService.DeleteAsync(id);
       return deleted.IsSuccess ? NoContent() : deleted.ToProblem();
    }
}
