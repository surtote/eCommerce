using FluentValidation;
using Identity.DTO;
using Identity.Models.Common;
using Identity.Services;

namespace Identity.Features.Users.V1
{
    public static class UpdateUser
    {
        public static RouteGroupBuilder MapUpdateUser(this RouteGroupBuilder group)
        {
            group.MapPut("/{userId}", HandleAsync)
                 .WithName("UpdateUserV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Update user information";
                     operation.Description = "Updates a user's information. Requires Admin role.";
                     return operation;
                 })
                 .Produces<UserResponse>(StatusCodes.Status200OK)
                 .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                 .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden)
                 .DisableAntiforgery();

            return group;
        }

        private static async Task<IResult> HandleAsync(
            string userId,
            UpdateUserRequest? request,
            IUserService userService,
            IValidator<UpdateUserRequest> validator,
            ILogger<UpdateUserRequest> logger,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = new[] { "Request body is required" },
                    Message = "Validation failed"
                });
            }

            // Validación con FluentValidation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
                logger.LogWarning("User update validation failed for {UserId}: {Errors}", userId, string.Join(", ", errors));
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = errors,
                    Message = "Validation failed"
                });
            }

            try
            {
                // Actualizar usuario
                var updatedUser = await userService.UpdateUserAsync(userId, request);

                if (updatedUser == null)
                {
                    logger.LogWarning("User not found: {UserId}", userId);
                    return Results.NotFound(new ErrorResponse
                    {
                        Errors = new[] { $"User {userId} not found" },
                        Message = "User not found"
                    });
                }

                logger.LogInformation("User updated successfully: {UserId}", userId);
                return Results.Ok(updatedUser);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update user {UserId}", userId);
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = new[] { ex.Message },
                    Message = "Failed to update user"
                });
            }
        }
    }
}
