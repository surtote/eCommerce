using Identity.DTO;
using Identity.Models;
using Identity.Models.Common;
using Identity.Services;

namespace Identity.Features.Roles.V1
{
    public static class GetRoleUsers
    {
        public static RouteGroupBuilder MapGetRoleUsers(this RouteGroupBuilder group)
        {
            group.MapGet("/{roleId}/users", HandleAsync)
                 .WithName("GetRoleUsersV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Get users in a role";
                     operation.Description = "Returns a paginated list of users assigned to a specific role. Requires Admin role.";
                     return operation;
                 })
                 .Produces<PaginatedResponse<Identity.DTO.UserResponse>>(StatusCodes.Status200OK) // ✅ Cambiado
                 .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden);

            return group;
        }

        private static async Task<IResult> HandleAsync(
            string roleId,
            [AsParameters] PaginationQuery pagination,
            IRolesService roleService,
            ILogger<PaginationQuery> logger)
        {
            logger.LogInformation("Fetching users for role: {RoleId}, Page: {Page}, PageSize: {PageSize}",
                roleId, pagination.Page, pagination.PageSize);

            var result = await roleService.GetUsersInRoleAsync(roleId, pagination);

            // Check if role was not found (empty result with no total count)
            if (result.Pagination.TotalCount == 0 && !result.Data.Any())
            {
                logger.LogWarning("Role not found: {RoleId}", roleId);
            }

            return Results.Ok(new PaginatedResponse<UserResponse>
            {
                Data = result.Data,
                Pagination = result.Pagination
            });
        }
    }
}