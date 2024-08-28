using SurveyBasketV3.Api.Contracts.Users;

namespace SurveyBasketV3.Api.Services
{
	public interface IUserService
	{
		Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
		Task<Result<UserResponse>> GetAsync(string id);
		Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
		Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
		Task<Result> ToggleStatusAsync(string id);
		Task<Result> UnlockAsync(string id);
		Task<Result<UserProfileResponse>> GetProfileAsync(string id);
		Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
		Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
	}
}
