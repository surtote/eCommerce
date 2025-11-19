using Identity.Models.Common;
using Identity.Services;


    namespace Identity.Features.Users.V1
    {
        public static class DeleteUser
        {
            public static RouteGroupBuilder MapDeleteUser(this RouteGroupBuilder group)
            {
                group.MapDelete("/{userId}", HandleAsync)
                     .WithName("DeleteUserV1")
                     .WithOpenApi(operation =>
                     {
                         operation.Summary = "Delete user";
                         operation.Description = "Permanently deletes a user account. Requires Admin role.";
                         return operation;
                     })
                     .Produces(StatusCodes.Status204NoContent)
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
                logger.LogInformation("Deleting user: {UserId}", userId);

                try
                {
                    var deleted = await userService.DeleteUserAsync(userId);

                    if (!deleted)
                    {
                        logger.LogWarning("User not found: {UserId}", userId);
                        return Results.NotFound(new ErrorResponse
                        {
                            Errors = new[] { $"User {userId} not found" },
                            Message = "User not found"
                        });
                    }

                    logger.LogInformation("User deleted successfully: {UserId}", userId);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to delete user {UserId}", userId);
                    return Results.BadRequest(new ErrorResponse
                    {
                        Errors = new[] { ex.Message },
                        Message = "Failed to delete user"
                    });
                }
            }
        }
    }

