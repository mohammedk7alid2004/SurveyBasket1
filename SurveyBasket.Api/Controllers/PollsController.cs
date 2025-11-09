using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Contract.Contracts.Poll;

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
        var polls = await _pollService.GetAllAsync(cancellationToken);
        return Ok(polls);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetAsync(id, cancellationToken);
        if (poll == null)
            return NotFound();

        return Ok(poll);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CreatePollRequest request, CancellationToken cancellationToken)
    {
        await _pollService.AddAsync(request, cancellationToken);

        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePollRequest request, CancellationToken cancellationToken)
    {
        var updated = await _pollService.UpdateAsync(id, request, cancellationToken);
        if (!updated)
            return NotFound();

        return NoContent();
    }
    [HttpPut("{id}/ToggelPublish")]
    public async Task<IActionResult> TogglePublishStatus(int id, CancellationToken cancellationToken)
    {
        var toggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        if (!toggled)
            return NotFound();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _pollService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
