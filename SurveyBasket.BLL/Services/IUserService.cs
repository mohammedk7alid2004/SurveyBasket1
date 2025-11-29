using SurveyBasket.Contract.Contracts.Users;

namespace SurveyBasket.BLL.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
    Task<Result>UpdateProfileAsync (string userId,UpdateUserProfileRequest request);
    Task<Result>ChangePasswordAsync(string userId,ChangePasswordRequest request);
  
}
