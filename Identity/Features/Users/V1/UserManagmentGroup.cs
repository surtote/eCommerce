using Asp.Versioning;

namespace Identity.Features.Users.V1
{
    public static class UserManagementGroup
    {
        /// <summary>
        /// Creates and configures the V1 admin user management API group with common settings.
        /// </summary>
        public static RouteGroupBuilder MapAdminUserGroup(this IEndpointRouteBuilder endpoints)
        {
            var versionSet = endpoints.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            return endpoints
                .MapGroup("/api/v{version:apiVersion}/admin/users")
                .WithApiVersionSet(versionSet)
                .WithTags("User Management")
                .RequireAuthorization(policy => policy.RequireRole(Data.Roles.Admin));
        }

        /// <summary>
        /// Creates and configures the V1 user self-management API group with common settings.
        /// </summary>
        public static RouteGroupBuilder MapUserSelfGroup(this IEndpointRouteBuilder endpoints)
        {
            var versionSet = endpoints.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            return endpoints
                .MapGroup("/api/v{version:apiVersion}/users")
                .WithApiVersionSet(versionSet)
                .WithTags("User Self-Management")
                .RequireAuthorization();
        }
    }
}
