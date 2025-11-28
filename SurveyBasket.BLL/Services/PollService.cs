using Hangfire;
using SurveyBasket.Contract.Contracts.Poll;

namespace SurveyBasket.BLL.Services;

public class PollService(ApplicationDbContext context ,INotificationsService notificationsService) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    private readonly INotificationsService _notificationsService = notificationsService;

    public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var polls = await _context.Polls
            .AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<PollResponse>>(polls);
    }
    public async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var polls = await _context.Polls
            .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)&& p.EndsAt>= DateOnly.FromDateTime(DateTime.UtcNow))
            .AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<PollResponse>>(polls);
    }
    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll= await _context.Polls
               .AsNoTracking()
               .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if ((poll is null))
            return Result.Failure<PollResponse>(PollErrors.PollNotFound);
        var pollResponse = poll.Adapt<PollResponse>();
        return Result.Success(pollResponse);
    }

    public async Task<Result> AddAsync(CreatePollRequest request, CancellationToken cancellationToken = default)
    {
        var isExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken: cancellationToken);
        if (isExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.PollAlreadyExists);
        var poll = request.Adapt<Poll>();
        if(poll is null)
            return Result.Failure(PollErrors.InvalidPollData);
        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<bool>> UpdateAsync(int id, UpdatePollRequest request, CancellationToken cancellationToken = default)
    {
        if(id<0)
            return Result.Failure<bool>(PollErrors.PollNotFound);
        var isExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title&&p.Id!=id, cancellationToken: cancellationToken);
        if (isExistingTitle)
            return Result.Failure<bool>(PollErrors.PollAlreadyExists);
        var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (poll == null)
            return Result.Failure<bool>(PollErrors.PollNotFound);

        request.Adapt(poll); 
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(int id,CancellationToken cancellationToken=default)
    {
        if (id < 0)
            return Result.Failure<bool>(PollErrors.PollNotFound);

        var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == id);
        if (poll == null)
            return Result.Failure<bool>(PollErrors.PollNotFound);

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync();
        return Result.Success(true);
    }

    public async Task<Result<bool>> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id < 0)
            return Result.Failure<bool>(PollErrors.PollNotFound);
        var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (poll == null)
            return Result.Failure<bool>(PollErrors.PollNotFound);
        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        if (poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => _notificationsService.SendNewPollNotificationAsync(poll.Id));
        return Result.Success(true);
    }
}
