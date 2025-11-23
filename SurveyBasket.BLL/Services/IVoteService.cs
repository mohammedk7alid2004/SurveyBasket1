using SurveyBasket.Contract.Contracts.Vote;

namespace SurveyBasket.BLL.Services;

public interface IVoteService
{
    Task<Result> AddAsync(int pollId,string userId, VoteRequest request, CancellationToken cancellationToken = default);
}
