using Microsoft.EntityFrameworkCore;
using SurveyBasket.BLL.Errors;
using SurveyBasket.Contract.Contracts.Questions;

namespace SurveyBasket.BLL.Services;

public class QuestionService(ApplicationDbContext context, ICashService cacheService) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ICashService _cacheService = cacheService;

    private const string CachePrefix = "availableQuestions";

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(
        int pollId,
        CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{CachePrefix}-{pollId}";
        var cachedQuestions = await _cacheService.GetAsync<IEnumerable<QuestionResponse>>(cacheKey, cancellationToken);

        if (cachedQuestions != null)
            return Result.Success(cachedQuestions);

        var questions = await _context.Questions
            .Where(q => q.PollId == pollId)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        await _cacheService.SetAsync(cacheKey, questions, cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(
        int pollId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var hasVoted = await _context.Votes
            .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteError.VoteAlreadyExists);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var pollExists = await _context.Polls.AnyAsync(
            p =>
                p.Id == pollId &&
                p.IsPublished &&
                p.StartsAt <= today &&
                p.EndsAt >= today,
            cancellationToken);

        if (!pollExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(q => q.PollId == pollId && q.IsActive)
            .Include(q => q.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers
                    .Where(a => a.IsActive)
                    .Select(a => new Contract.Contracts.Answers.AnswerResponse(a.Id, a.Content))
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<QuestionResponse>> GetByIdAsync(
        int pollId,
        int questionId,
        CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == questionId)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (question == null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }

    public async Task<Result<QuestionResponse>> AddAsync(
        int pollId,
        QuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionExists = await _context.Questions
            .AnyAsync(q =>
                q.PollId == pollId &&
                q.Content.ToLower().Trim() == request.Content.ToLower().Trim(),
                cancellationToken);

        if (questionExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionAlreadyExists);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        question.Answers = question.Answers
            .GroupBy(a => a.Content.Trim().ToLower())
            .Select(g => g.First())
            .ToList();

        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        var response = question.Adapt<QuestionResponse>();
        return Result.Success(response);
    }

    public async Task<Result<bool>> TogglePublishStatusAsync(
        int pollId,
        int questionId,
        CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure<bool>(PollErrors.PollNotFound);

        var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == pollId, cancellationToken);
        if (poll == null)
            return Result.Failure<bool>(PollErrors.PollNotFound);

        var question = await _context.Questions
            .SingleOrDefaultAsync(q => q.Id == questionId && q.PollId == pollId, cancellationToken);

        if (question == null)
            return Result.Failure<bool>(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success(true);
    }

    public async Task<Result> UpdateAsync(
        int pollId,
        int questionId,
        QuestionUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.Questions.AnyAsync(
            q =>
                q.PollId == pollId &&
                q.Id != questionId &&
                q.Content == request.Content,
            cancellationToken);

        if (exists)
            return Result.Failure(QuestionErrors.QuestionAlreadyExists);

        var question = await _context.Questions
            .Include(q => q.Answers)
            .SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == questionId, cancellationToken);

        if (question == null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        var currentAnswers = question.Answers.Select(x => x.Content).ToList();
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        foreach (var answer in newAnswers)
        {
            question.Answers.Add(new Answer { Content = answer });
        }

        foreach (var ans in question.Answers)
        {
            ans.IsActive = request.Answers.Contains(ans.Content);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }
}
