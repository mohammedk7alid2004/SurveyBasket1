
using SurveyBasket.Contract.Contracts.Poll;

namespace SurveyBasket.BLL.Services;

public interface IPollService
{
    Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken=default);
    Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task <Result>AddAsync (CreatePollRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync (int id , UpdatePollRequest poll, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id,CancellationToken cancellationToken =default);
    Task<Result<bool>> TogglePublishStatusAsync(int id,CancellationToken cancellationToken=default);
}
