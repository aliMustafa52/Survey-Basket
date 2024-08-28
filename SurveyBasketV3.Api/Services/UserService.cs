﻿using SurveyBasketV3.Api.Contracts.Users;
using SurveyBasketV3.Api.Persistence;

namespace SurveyBasketV3.Api.Services
{
	public class UserService(ApplicationDbContext context,
		IRoleService roleService,
		UserManager<ApplicationUser> userManager) : IUserService
	{
		private readonly ApplicationDbContext _context = context;
		private readonly IRoleService _roleService = roleService;
		private readonly UserManager<ApplicationUser> _userManager = userManager;

		public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) 
			=>
				await (from u in _context.Users
				 join ur in _context.UserRoles
				 on u.Id equals ur.UserId
				 join r in _context.Roles
				 on ur.RoleId equals r.Id into roles
				 where roles.Any(x => x.Name != DefaultRoles.Member)
				 select new
					 {
						 u.Id,
						 u.FirstName,
						 u.LastName,
						 u.Email,
						 u.IsDisabled,
						 Roles = roles.Select(x => x.Name).ToList()
					 }
				 )
			.GroupBy(x => new {x.Id, x.FirstName, x.LastName,  x.Email, x.IsDisabled})
			.Select(u => new UserResponse(
				u.Key.Id,
				u.Key.FirstName,
				u.Key.LastName,
				u.Key.Email,
				u.Key.IsDisabled,
				u.SelectMany(x => x.Roles))
			)
			.ToListAsync(cancellationToken);

		public async Task<Result<UserResponse>> GetAsync(string id)
		{
			if (await _userManager.FindByIdAsync(id) is not { } user)
				return Result.Failure<UserResponse>(UserErrors.UserNotFound);

			var userRoles = await _userManager.GetRolesAsync(user);

			//var userResponse = new UserResponse(user.Id, user.FirstName, user.LastName, user.Email!, user.IsDisabled, userRoles);
			var userResponse = (user, userRoles).Adapt<UserResponse>();

			return Result.Success(userResponse);
		}

		public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
		{
			var isEmailExisted = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
			if (isEmailExisted)
				return Result.Failure<UserResponse>(UserErrors.ExistedEmail);

			var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);
			if(request.Roles.Except(allowedRoles.Select(x => x.Name).ToList()).Any())
				return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

			var user = request.Adapt<ApplicationUser>();

			var result = await _userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			await _userManager.AddToRolesAsync(user, request.Roles);

			var userResponse = (user, request.Roles).Adapt<UserResponse>();
			return Result.Success(userResponse);
		}

		public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
		{
			var isEmailExisted = await _userManager.Users
							.AnyAsync(x => x.Email == request.Email && x.Id != id, cancellationToken);
			if (isEmailExisted)
				return Result.Failure(UserErrors.ExistedEmail);

			var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);
			if (request.Roles.Except(allowedRoles.Select(x => x.Name).ToList()).Any())
				return Result.Failure(UserErrors.InvalidRoles);

			if(await _userManager.FindByIdAsync(id) is not { } user)
				return Result.Failure(UserErrors.UserNotFound);

			user = request.Adapt(user);

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			await _context.UserRoles
					.Where(x => x.UserId == id)
					.ExecuteDeleteAsync(cancellationToken);

			await _userManager.AddToRolesAsync(user, request.Roles);

			return Result.Success();
		}

		public async Task<Result> ToggleStatusAsync(string id)
		{
			if (await _userManager.FindByIdAsync(id) is not { } user)
				return Result.Failure(UserErrors.UserNotFound);

			user.IsDisabled = !user.IsDisabled;
			var result =  await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			return Result.Success();
		}

		public async Task<Result> UnlockAsync(string id)
		{
			if(await _userManager.FindByIdAsync(id) is not { } user)
				return Result.Failure(UserErrors.UserNotFound);

			var result = await _userManager.SetLockoutEndDateAsync(user, null);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			return Result.Success();
		}
		public async Task<Result<UserProfileResponse>> GetProfileAsync(string id)
		{
			var userProfileResponse = await _userManager.Users
					.Where(x => x.Id == id)
					.ProjectToType<UserProfileResponse>()
					//.Select(x => new UserProfileResponse(x.Email!, x.UserName!, x.FirstName, x.LastName))
					.SingleAsync();

			return Result.Success(userProfileResponse);
		}

		public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
		{
			//var user = await _userManager.FindByIdAsync(userId);
			//user = request.Adapt(user);
			//await _userManager.UpdateAsync(user!);

			await _userManager.Users
				.Where(x => x.Id == userId)
				.ExecuteUpdateAsync(setters =>
					setters
						.SetProperty(x => x.FirstName, request.FirstName)
						.SetProperty(x => x.LastName, request.LastName)
				);

			return Result.Success();
		}

		public async Task<Result> ChangePasswordAsync(string userId,ChangePasswordRequest request)
		{
			var user = await _userManager.FindByIdAsync(userId);

			var result =  await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

			}

			return Result.Success();
		}

		
	}
}
