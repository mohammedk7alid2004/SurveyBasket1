using SurveyBasket.Contract.Contracts.Questions;

namespace SurveyBasket.Api.Controllers;

[Route("api/poll/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService service) : ControllerBase
{
    private readonly IQuestionService _service = service;
    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromRoute] int pollId , CancellationToken cancellationToken)
    {
        var result =await _service.GetAllAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("{questionId}")]
    public async Task<IActionResult> Get([FromRoute]int pollId , [FromRoute]int questionId ,CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(pollId, questionId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("")]
    public async Task<IActionResult>Add([FromRoute] int pollId, [FromBody]QuestionRequest request , CancellationToken cancellationToken)
    {
         var result = await _service.AddAsync(pollId, request , cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(
                nameof(Get),
                new { pollId = pollId, questionId = result.Value.Id },
                result.Value
              )
            : result.ToProblem();
    }
    [HttpPut("{questionId}")]
    public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int questionId, [FromBody] QuestionUpdateRequest request , CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(pollId, questionId, request , cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPut("{questionId}/toggle-publish")]
    public async Task<IActionResult> TogglePublishStatus([FromRoute] int pollId, [FromRoute] int questionId , CancellationToken cancellationToken)
    {
        var result = await _service.TogglePublishStatusAsync(pollId, questionId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
