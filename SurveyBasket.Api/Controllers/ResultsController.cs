using Microsoft.AspNetCore.Http;
namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class ResultsController(IResultService result) : ControllerBase
{
    private readonly IResultService _result = result;

[HttpGet("row-data")]
public async Task<IActionResult> PollVotes([FromRoute] int pollId ,CancellationToken cancellationToken)

    {
        var result = await _result.GetPollVotesAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            
   }
    [HttpGet("votes-per-day")]
    public async Task<IActionResult> VotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _result.GetVotesPerDayAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("votes-per-question")]
    public async Task<IActionResult> VotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _result.GetVotesPerQuestionAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}