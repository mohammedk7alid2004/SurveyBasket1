using SurveyBasket.Contract.Contracts.Questions;

namespace SurveyBasket.BLL.Services;
public interface IQuestionService
{
    Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId,CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId,string userId,  CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetByIdAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
    Task<Result<bool>> TogglePublishStatusAsync(int pollId,int questionId, CancellationToken cancellationToken = default);
    Task<Result>UpdateAsync(int pollId, int questionId, QuestionUpdateRequest request,CancellationToken cancellationToken = default);
}
