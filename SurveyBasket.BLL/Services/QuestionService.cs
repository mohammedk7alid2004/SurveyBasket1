using Microsoft.EntityFrameworkCore;
using SurveyBasket.BLL.Errors;
using SurveyBasket.Contract.Contracts.Questions;

namespace SurveyBasket.BLL.Services;

public class QuestionService(ApplicationDbContext contex) : IQuestionService
{
    private readonly ApplicationDbContext _contex = contex;
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {

        if (pollId <= 0)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
        var pollIsExist = await _contex.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable< QuestionResponse>>(PollErrors.PollNotFound);
        var result = await _contex.Questions
            .Where(x => x.PollId == pollId)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<QuestionResponse>>(result);
    }
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(
     int pollId,
     string userId,
     CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var hasVoted = await _contex.Votes
            .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteError.VoteAlreadyExists);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var pollExists = await _contex.Polls
            .AnyAsync(p =>
                p.Id == pollId &&
                p.IsPublished &&
                p.StartsAt <= today &&
                p.EndsAt >= today,
                cancellationToken);

        if (!pollExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _contex.Questions
            .Where(q => q.PollId == pollId && q.IsActive)
            .Include(q => q.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers
                    .Where(a => a.IsActive)
                    .Select(a => new Contract.Contracts.Answers.AnswerResponse(
                        a.Id,
                        a.Content
                    ))
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<QuestionResponse>> GetByIdAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {
       
        if (pollId <= 0)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var pollIsExist = await _contex.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);
        var question = await _contex.Questions.Where(x => x.PollId == pollId && x.Id == questionId)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (question == null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
        return Result.Success<QuestionResponse>(question);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        if (pollId <= 0)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var pollIsExist = await _contex.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionExists = await _contex.Questions
            .AnyAsync(q => q.Content.ToLower() == request.Content.ToLower().Trim()
                        && q.PollId == pollId, cancellationToken);

        if (questionExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionAlreadyExists);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        question.Answers = question.Answers
            .GroupBy(a => a.Content.Trim().ToLower())
            .Select(g => g.First())
            .ToList();

        await _contex.Questions.AddAsync(question, cancellationToken);
        await _contex.SaveChangesAsync(cancellationToken);

        var response = question.Adapt<QuestionResponse>();
        return Result.Success(response);
    }

    public  async Task<Result<bool>> TogglePublishStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {

        if (pollId < 0)
            return Result.Failure<bool>(PollErrors.PollNotFound);
        var poll = await _contex.Polls.SingleOrDefaultAsync(p => p.Id == pollId, cancellationToken);
        if (poll == null)
            return Result.Failure<bool>(PollErrors.PollNotFound);
        var question = await _contex.Questions.SingleOrDefaultAsync(p => p.Id == questionId && p.PollId == pollId, cancellationToken);
        if (question == null)
            return Result.Failure<bool>(QuestionErrors.QuestionNotFound);
        question.IsActive = !question.IsActive;
        await _contex.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result> UpdateAsync(int pollId, int questionId,QuestionUpdateRequest request, CancellationToken cancellationToken = default)
    {
      var questionIsExist= await _contex .Questions
            .AnyAsync(q=>q.PollId == pollId &&
            q.Id!= questionId
            &&q.Content==request.Content
            ,cancellationToken);
        if (questionIsExist)
            return Result.Failure(QuestionErrors.QuestionAlreadyExists);
        var question = await _contex.Questions.Include(x=>x.Answers)
            .SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == questionId, cancellationToken);
        if (question == null)
            return Result.Failure(QuestionErrors.QuestionNotFound);
        question.Content = request.Content;
        var currentAnswers = question.Answers.Select(x=>x.Content).ToList();
        var newAnswers= request.Answers.Except(currentAnswers).ToList();
        newAnswers.ForEach(answerContent =>
        {
            question.Answers.Add(new Answer { Content = answerContent });
        });
        question.Answers.ToList().ForEach(ans =>
        {
            ans.IsActive = request.Answers.Contains(ans.Content);
        });
        await _contex.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

   
}
