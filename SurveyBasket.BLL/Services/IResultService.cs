using SurveyBasket.Contract.Contracts.Results;

namespace SurveyBasket.BLL.Services;

public interface IResultService
{
    Task<Result<PollVoteReponse>> GetPollVotesAsync(int? pollId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int? pollId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<VotePerQuestionResponse>>>GetVotesPerQuestionAsync(int? pollId, CancellationToken cancellationToken);
}