using Identity.DTO;
using Identity.Models.Common;
using Identity.Services;

namespace Identity.Features.Users.V1
{
    public static class GetUserById
    {
        public static RouteGroupBuilder MapGetUserById(this RouteGroupBuilder group)
        {
            group.MapGet("/{userId}", HandleAsync)
                 .WithName("GetUserByIdV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Get user by ID";
                     operation.Description = "Returns detailed information about a specific user. Requires Admin role.";
                     return operation;
                 })
                 .Produces<UserResponse>(StatusCodes.Status200OK)
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
            logger.LogInformation("Fetching user details for: {UserId}", userId);

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Results.NotFound(new ErrorResponse
                {
                    Errors = new[] { "User not found" },
                    Message = "User not found"
                });
            }

            return Results.Ok(user);
        }

    }
}