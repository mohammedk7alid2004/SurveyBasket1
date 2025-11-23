using System.Security.Claims;
using SurveyBasket.Api.Extensions;
using SurveyBasket.Contract.Contracts.Vote;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService service ,IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _service = service;
    private readonly IVoteService _voteService = voteService;

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await _service.GetAvailableAsync(pollId, userId!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }
    [HttpPost("")]
    public async Task<IActionResult> Vote(
        [FromRoute] int pollId,
        [FromBody] VoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _voteService.AddAsync(pollId, userId!, request, cancellationToken);
        return result.IsSuccess ? Created() : result.ToProblem();
    }
}
