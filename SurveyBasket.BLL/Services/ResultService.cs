
using SurveyBasket.Contract.Contracts.Results;

namespace SurveyBasket.BLL.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVoteReponse>> GetPollVotesAsync(int? pollId, CancellationToken cancellationToken)
    {
        var pollVotes = await _context.Polls
             .Select(x => new PollVoteReponse(

                 x.Title,
                 x.Votes.Select(v => new VoteResponse
                 (
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(ans => new QuestionAnswerResponse
                    (
                        ans.Question.Content,
                        ans.Answer.Content
                    )
                 )
             )))).SingleOrDefaultAsync(cancellationToken);
        return pollVotes is null
            ? Result.Failure<PollVoteReponse>(PollErrors.PollNotFound) :
            Result.Success(pollVotes);
    }

    public async Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int? pollId, CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<VotePerDayResponse>>(PollErrors.PollNotFound);
        var votePerDay = await _context.Votes
            .Where(x => x.PollId == pollId)
            .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
            .Select(x => new VotePerDayResponse
            (
             x.Key.Date,
             x.Count()
            )
            ).ToListAsync(cancellationToken);


        return Result.Success<IEnumerable<VotePerDayResponse>>(votePerDay);
    }

    public async Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotesPerQuestionAsync(int? pollId, CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<VotePerQuestionResponse>>(PollErrors.PollNotFound);
        var votePerQuestion = await _context.VotesAnswers
     .Where(va => va.Vote.PollId == pollId)
     .GroupBy(va => new
     {
         QuestionId = va.QuestionId,
         QuestionContent = va.Question.Content
     })
     .Select(g => new VotePerQuestionResponse(
         g.Key.QuestionContent,
         g.GroupBy(a => new
         {
             AnswerId = a.AnswerId,
             AnswerContent = a.Answer.Content
         })
         .Select(a => new VotePerAnswerReponse(
             a.Key.AnswerContent,
             a.Count()
         ))
         .ToList()
     ))
     .AsNoTracking()
     .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotePerQuestionResponse>>(votePerQuestion);
    }
}
