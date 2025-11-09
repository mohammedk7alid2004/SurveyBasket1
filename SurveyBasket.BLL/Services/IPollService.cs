
using SurveyBasket.Contract.Contracts.Poll;

namespace SurveyBasket.BLL.Services;

public interface IPollService
{
   Task< IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken=default);
    Task<Poll> GetAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync (CreatePollRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync (int id , UpdatePollRequest poll, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id,CancellationToken cancellationToken =default);
    Task <bool>TogglePublishStatusAsync(int id,CancellationToken cancellationToken=default);
}
