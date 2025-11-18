using FluentValidation;
using Identity.Models.Common;
using Identity.Models.Roles.Request;
using Identity.Services;

namespace Identity.Features.Roles.V1
{
    public static class UpdateRole
    {
        public static RouteGroupBuilder MapUpdateRole(this RouteGroupBuilder group)
        {
            group.MapPut("/{roleId}", HandleAsync)
                 .WithName("UpdateRoleV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Update a role";
                     operation.Description = "Updates an existing role's name. Requires Admin role.";
                     return operation;
                 })
                 .Produces<Models.Roles.Responses.RoleResponse>(StatusCodes.Status200OK)
                 .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                 .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden)
                 .DisableAntiforgery();

            return group;
        }


        private static async Task<IResult> HandleAsync(
            string roleId,
            UpdateRoleRequest? request,
            IRolesService roleService,
            IValidator<UpdateRoleRequest> validator,
            ILogger<UpdateRoleRequest> logger,
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
                logger.LogWarning("Role update validation failed for: {RoleId}", roleId);
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = errors,
                    Message = "Validation failed"
                });
            }

            logger.LogInformation("Updating role {RoleId} to name: {NewName}", roleId, request.RoleName);

            var result = await roleService.UpdateRoleAsync(roleId, request.RoleName);

            if (!result.Succeeded)
            {
                logger.LogWarning("Role update failed for {RoleId}: {Errors}",
                    roleId, string.Join(", ", result.Errors));

                // Check if it's a not found error
                if (result.Errors.Any(e => e.Contains("not found") || e.Contains("does not exist")))
                {
                    return Results.NotFound(new ErrorResponse
                    {
                        Errors = result.Errors,
                        Message = "Role not found"
                    });
                }

                return Results.BadRequest(new ErrorResponse
                {
                    Errors = result.Errors,
                    Message = "Failed to update role"
                });
            }

            logger.LogInformation("Role updated successfully: {RoleId}", roleId);

            return Results.Ok(result.Data);
        }
    }
}
