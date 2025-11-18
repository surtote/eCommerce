using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Identity.Features.Roles.V1
{
    public static class RoleManagementGroup
    {
        /// <summary>
        /// Creates and configures the V1 role management API group with common settings.
        /// </summary>
        public static RouteGroupBuilder MapRoleManagementGroup(this IEndpointRouteBuilder endpoints)
        {
            var versionSet = endpoints.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            return endpoints
                .MapGroup("/api/admin/roles")
                .WithApiVersionSet(versionSet)
                .WithTags("Role Management")
                .RequireAuthorization(policy => policy.RequireRole(Data.Roles.Admin));
        }
    }
}
