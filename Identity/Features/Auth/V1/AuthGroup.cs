using Asp.Versioning;

namespace Identity.Features.Auth.V1
{
    public static class AuthGroup
    {
        /// <summary>
        /// Creates and configures the V1 authentication API group with common settings.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder</param>
        /// <returns>Configured route group builder for V1 auth endpoints</returns>
        public static RouteGroupBuilder MapAuthGroup(this IEndpointRouteBuilder endpoints)
        {
            var versionSet = endpoints.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            return endpoints
                .MapGroup("/api/v{version:apiVersion}/auth")
                .WithApiVersionSet(versionSet)
                .WithTags("Authentication V1");
        }
    }
}