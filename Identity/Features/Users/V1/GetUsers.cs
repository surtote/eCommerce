using Identity.DTO;
using Identity.Models.Common;
using Identity.Models.Query;
using Identity.Services;

namespace Identity.Features.Users.V1
{
    public static class GetUsers
    {
        public static RouteGroupBuilder MapGetUsers(this RouteGroupBuilder group)
        {
            group.MapGet("/", HandleAsync)
                 .WithName("GetUsersV1")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Get all users";
                     operation.Description = "Returns a paginated list of users with optional filtering and sorting. Requires Admin role.";
                     return operation;
                 })
                 .Produces<PaginatedResponse<UserResponse>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden);

            return group;
        }

        private static async Task<IResult> HandleAsync(
            [AsParameters] UserQueryParameters query,
            IUserService userService,
            ILogger<UserQueryParameters> logger)
        {
            logger.LogInformation("Fetching users: Page {Page}, PageSize {PageSize}, Search {Search}, Role {Role}",
                query.Page, query.PageSize, query.Search ?? "none", query.Role ?? "none");

            var result = await userService.GetUsersAsync(query);

            var response = new PaginatedResponse<UserResponse>
            {
                Data = result.Data,
                Pagination = result.Pagination
            };

            return Results.Ok(response);
        }
    }
}
