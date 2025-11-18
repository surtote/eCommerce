using Identity.Models.Common;
using Identity.Services;

namespace Identity.Features.Roles.V1
{
    public static class GetRoles
    {
        public static RouteGroupBuilder MapGetRoles(this RouteGroupBuilder group)
        {
            group.MapGet("/", HandleAsync)
                 .WithName("GetRolesV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Get all roles";
                     operation.Description = "Returns a list of all roles in the system. Requires Admin role.";
                     return operation;
                 })
                 .Produces<IEnumerable<Models.Roles.Responses.RoleResponse>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden);

            return group;
        }


        private static async Task<IResult> HandleAsync(
            IRolesService roleService,
            ILogger<IRolesService> logger)
        {
            logger.LogInformation("Fetching all roles");

            var result = await roleService.GetAllRolesAsync();

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to fetch roles: {Errors}",
                    string.Join(", ", result.Errors));
                return Results.BadRequest(new ErrorResponse
                {
                    Errors = result.Errors,
                    Message = "Failed to fetch roles"
                });
            }

            return Results.Ok(result.Data);
        }
    }
}
