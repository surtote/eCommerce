using Identity.Models.Common;
using Identity.Services;

namespace Identity.Features.Users.V1
{
    public static class GetUserRoles
    {
        public static RouteGroupBuilder MapGetUserRoles(this RouteGroupBuilder group)
        {
            group.MapGet("/{userId}/roles", HandleAsync)
                 .WithName("GetUserRolesV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Get user's roles";
                     operation.Description = "Returns all roles assigned to a user. Requires Admin role.";
                     return operation;
                 })
                 .Produces<IEnumerable<string>>(StatusCodes.Status200OK)
                 .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden);

            return group;
        }

        private static async Task<IResult> HandleAsync(
            string userId,
            IUserService userService,
            ILogger<string> logger)
        {
            logger.LogInformation("Fetching roles for user: {UserId}", userId);

            var result = await userService.GetUserRolesAsync(userId);

            if (!result.Succeeded)
            {
                logger.LogWarning("Get user roles failed for {UserId}: {Errors}",
                    userId, string.Join(", ", result.Errors));

                return Results.NotFound(new ErrorResponse
                {
                    Errors = result.Errors,
                    Message = "User not found"
                });
            }

            return Results.Ok(new { UserId = userId, Roles = result.Data });
        }
    }
}
