using Asp.Versioning;

namespace Identity.Features.Users.V1
{
    public static class UserManagementGroup
    {
        /// ADMIN USER MANAGEMENT
        public static RouteGroupBuilder MapAdminUserGroup(this IEndpointRouteBuilder endpoints)
        {
            var versionSet = endpoints.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            return endpoints
                .MapGroup("/api/v{version:apiVersion}/admin/users")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(new ApiVersion(1, 0))   // 🔥 NECESARIO
                .WithTags("Admin User Management")
                .RequireAuthorization(policy => policy.RequireRole(Data.Roles.Admin));
        }

        /// USER SELF-MANAGEMENT
        public static RouteGroupBuilder MapUserSelfGroup(this IEndpointRouteBuilder endpoints)
        {
            var versionSet = endpoints.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            return endpoints
                .MapGroup("/api/v{version:apiVersion}/users")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(new ApiVersion(1, 0))   // 🔥 NECESARIO
                .WithTags("User Self-Management")
                .RequireAuthorization();
        }
    }
}
