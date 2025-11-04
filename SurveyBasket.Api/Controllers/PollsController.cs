using Mapster;
using SurveyBasket.BLL.Services;
using SurveyBasket.Contract.Contracts.Request;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
               return Ok(_pollService.GetAll());
    }
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var poll= _pollService.Get(id);
        if(poll == null)
            return NotFound();
        return Ok(poll);
    }
    [HttpPost]
    public IActionResult Add (CreatePollRequest request)
    {
        var poll = request.Adapt<Poll>();
        var npoll = _pollService.Add(poll);
        return CreatedAtAction(nameof(Get),new {id = npoll.Id} ,npoll);
    }
    [HttpPut("{id}")]
    public IActionResult Update (int id, Poll poll)
    {
      var updated = _pollService.Update(id, poll);
        if(!updated)
            return NotFound();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _pollService.Delete(id);
        if(!deleted)
            return NotFound();
        return NoContent();
    }
}
