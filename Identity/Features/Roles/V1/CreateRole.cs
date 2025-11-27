using FluentValidation;
using Identity.Models.Common;
using Identity.Models.Roles.Request;
using Identity.Models.Roles.Responses;
using Identity.Services;

namespace Identity.Features.Roles.V1
{
    public static class CreateRole
    {
        public static RouteGroupBuilder MapCreateRole(this RouteGroupBuilder group)
        {
            group.MapPost("/", HandleAsync)
                .WithName("CreateRoleV1")
                .AddOpenApiOperationTransformer((operation, context, ct) =>
                {
                    operation.Summary = "Create a new role";
                    operation.Description = "Creates a new role in the system. Requires Admin role.";
                    return Task.CompletedTask;
                })
                .Produces<Models.Roles.Responses.RoleResponse>(StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .DisableAntiforgery();

            return group;
        }


        private static async Task<IResult> HandleAsync(
            CreateRoleRequest request,
            IRolesService roleService,
            IValidator<CreateRoleRequest> validator)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = validation.Errors.Select(e => e.ErrorMessage)
                });
            }

            var result = await roleService.CreateRoleAsync(request.RoleName);

            if (!result.Succeeded)
            {
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = result.Errors
                });
            }

            return Results.Created($"/api/v1/admin/roles/{result.Data!.RoleId}", result.Data);
        }
    }
}
