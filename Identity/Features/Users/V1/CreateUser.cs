using FluentValidation;
using Identity.DTO;
using Identity.Models.Common;
using Identity.Services;

namespace Identity.Features.Users.V1
{
    public static class CreateUser
    {
        public static RouteGroupBuilder MapCreateUser(this RouteGroupBuilder group)
        {
            group.MapPost("/", HandleAsync)
                 .WithName("CreateUserV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Create a new user";
                     operation.Description = "Creates a new user account with specified roles. Requires Admin role.";
                     return operation;
                 })
                 .Produces<UserResponse>(StatusCodes.Status201Created)
                 .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden)
                 .DisableAntiforgery();

            return group;
        }


        private static async Task<IResult> HandleAsync(
        CreateUserRequest? request,
        IUserService userService,
        IValidator<CreateUserRequest> validator,
        ILogger<CreateUserRequest> logger,
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

            // Validación
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
                logger.LogWarning("User creation validation failed: {Errors}", string.Join(", ", errors));
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = errors,
                    Message = "Validation failed"
                });
            }

            try
            {
                // Crear usuario
                var createdUser = await userService.CreateUserAsync(request);

                logger.LogInformation("User created successfully: {UserId}", createdUser.Id);

                return Results.Created($"/api/v1/admin/users/{createdUser.Id}", createdUser);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create user");
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = new[] { ex.Message },
                    Message = "Failed to create user"
                });
            }
        }
    }
}
