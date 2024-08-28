using Microsoft.AspNetCore.Identity;
using SurveyBasketV3.Api.Contracts.Roles;
using SurveyBasketV3.Api.Persistence;
using System.Linq;

namespace SurveyBasketV3.Api.Services
{
	public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context) : IRoleService
	{
		private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
		private readonly ApplicationDbContext _context = context;

		public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisable = false,
			CancellationToken cancellationToken = default)
		{
			var response = await _roleManager.Roles
					.Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisable.HasValue && includeDisable.Value)))
					.ProjectToType<RoleResponse>()
					.ToListAsync(cancellationToken);

			return response;
		}

		public async Task<Result<RoleDetailsResponse>> GetAsync(string id)
		{

			if(await  _roleManager.FindByIdAsync(id) is not { } role)
				return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

			var permissions = await _roleManager.GetClaimsAsync(role);

			var roleDetailsResponse = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));

			return Result.Success(roleDetailsResponse);

			

		}

		public async Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request)
		{

			var isRoleExisted = await _roleManager.RoleExistsAsync(request.Name);
			if (isRoleExisted)
				return Result.Failure<RoleDetailsResponse>(RoleErrors.DublicatedRole);

			var allowedPermissions = Permissions.GetAllPermissions();
			if(request.Permissions.Except(allowedPermissions).Any())
				return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermission);

			var role = new ApplicationRole
			{
				Name = request.Name,
				ConcurrencyStamp = Guid.NewGuid().ToString(),
			};
			var result = await _roleManager.CreateAsync(role);
			if(!result.Succeeded)
			{
				var error = result.Errors.First();
				Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			var permissions = request.Permissions.Select(x => new IdentityRoleClaim<string>
			{
				ClaimType = Permissions.Type,
				ClaimValue = x,
				RoleId = role.Id,
			});

			await _context.RoleClaims.AddRangeAsync(permissions);
			await _context.SaveChangesAsync();

			var roleDetailsResponse = new RoleDetailsResponse(role.Id,role.Name,role.IsDeleted, request.Permissions);
			return Result.Success(roleDetailsResponse);
		}

		public async Task<Result> UpdateAsync(string id, RoleRequest request)
		{
			var isRoleExisted = _roleManager.Roles.Any(x => x.Name == request.Name && x.Id != id);
			if (isRoleExisted)
				return Result.Failure<RoleDetailsResponse>(RoleErrors.DublicatedRole);

			if(await _roleManager.FindByIdAsync(id) is not { } role)
				return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

			var allowedPermissions = Permissions.GetAllPermissions();
			if (request.Permissions.Except(allowedPermissions).Any())
				return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermission);

			role.Name = request.Name;

			var result= await _roleManager.UpdateAsync(role);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			var currentPermissions = await _context.RoleClaims
					.Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
					.Select(x => x.ClaimValue)
					.ToListAsync();

			var newPermissions = request.Permissions.Except(currentPermissions)
					.Select(x => new IdentityRoleClaim<string>
					{
						ClaimType = Permissions.Type,
						ClaimValue = x,
						RoleId = id,
					});

			var removedPermissions =currentPermissions.Except(request.Permissions);

			await _context.RoleClaims
				.Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
				.ExecuteDeleteAsync();

			await _context.AddRangeAsync(newPermissions);
			await _context.SaveChangesAsync();

			return Result.Success();
		}

		public async Task<Result> ToggleStatusAsync(string id)
		{
			if (await _roleManager.FindByIdAsync(id) is not { } role)
				return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

			role.IsDeleted = !role.IsDeleted;

			await _roleManager.UpdateAsync(role);

			return Result.Success();
		}
	}
}
