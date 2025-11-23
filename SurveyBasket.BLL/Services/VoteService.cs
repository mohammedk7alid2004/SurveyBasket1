using SurveyBasket.Contract.Contracts.Vote;

namespace SurveyBasket.BLL.Services;

public class VoteService(ApplicationDbContext context): IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async  Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure(PollErrors.PollNotFound);

        var hasVoted = await _context.Votes
            .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVoted)
            return Result.Failure(VoteError.VoteAlreadyExists);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var pollExists = await _context.Polls
          .AnyAsync(p =>
              p.Id == pollId &&
              p.IsPublished &&
              p.StartsAt <= today &&
              p.EndsAt >= today,
              cancellationToken);

        if (!pollExists)
            return Result.Failure(PollErrors.PollNotFound);
        var availableQuestion = await _context.Questions
            .Where(q => q.PollId == pollId&& q.IsActive)
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);
        if(!request.Answers.Select(x=>x.QuestionId).SequenceEqual(availableQuestion))
            return Result.Failure(VoteError.InvalidVoteData);
        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            SubmittedOn= DateTime.UtcNow,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };
        await _context.Votes.AddAsync(vote, cancellationToken);
        await  _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
