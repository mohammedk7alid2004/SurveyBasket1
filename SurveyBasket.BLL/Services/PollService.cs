using SurveyBasket.Contract.Contracts.Poll;

namespace SurveyBasket.BLL.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Polls
               .AsNoTracking()
               .ToListAsync(cancellationToken);
    }

    public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Polls
               .AsNoTracking()
               .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(CreatePollRequest request, CancellationToken cancellationToken = default)
    {
        var poll = request.Adapt<Poll>();
        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePollRequest request, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (poll == null)
            return false;

        request.Adapt(poll); 
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id,CancellationToken cancellationToken=default)
    {
        var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == id);
        if (poll == null)
            return false;

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (poll == null)
            return false;
        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
