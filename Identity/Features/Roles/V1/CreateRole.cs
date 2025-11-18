using FluentValidation;
using Identity.Models.Common;
using Identity.Models.Roles.Request;
using Identity.Services;
using Microsoft.AspNetCore.OpenApi;
namespace Identity.Features.Roles.V1
{
    public static class CreateRole
    {
        public static RouteGroupBuilder MapCreateRole(this RouteGroupBuilder group)
        {
            group.MapPost("/", HandleAsync)
                 .WithName("CreateRoleV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Create a new role";
                     operation.Description = "Creates a new role in the system. Requires Admin role.";
                     return operation;
                 })
                 .Produces<Models.Roles.Responses.RoleResponse>(StatusCodes.Status201Created)
                 .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden)
                 .DisableAntiforgery();

            return group;
        }

        private static async Task<IResult> HandleAsync(
            CreateRoleRequest? request,
            IRolesService roleService,
            IValidator<CreateRoleRequest> validator,
            ILogger<CreateRoleRequest> logger,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = ["Request body is required"]
                });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                logger.LogWarning("Role creation validation failed for: {RoleName}", request.RoleName);
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = errors,
                    Message = "Validation failed"
                });
            }

            logger.LogInformation("Creating role: {RoleName}", request.RoleName);

            var result = await roleService.CreateRoleAsync(request.RoleName);

            if (!result.Succeeded)
            {
                logger.LogWarning("Role creation failed for {RoleName}: {Errors}",
                    request.RoleName, string.Join(", ", result.Errors));
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = result.Errors,
                    Message = "Failed to create role"
                });
            }

            logger.LogInformation("Role created successfully: {RoleName}", request.RoleName);

            return Results.Created($"/api/v1/admin/roles/{result.Data!.RoleId}", result.Data);
        }
    }
}
