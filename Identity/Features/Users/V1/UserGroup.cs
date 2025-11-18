namespace Identity.Features.Users.V1
{
    public static class UserGroup
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
        {
            // Admin - User Management (grouped under "User Management" tag)
            var adminGroup = endpoints.MapAdminUserGroup();
            adminGroup.MapGetUsers();
            adminGroup.MapGetUserById();
          

            // Self-Management (grouped under "User Self-Management" tag)
            //var selfGroup = endpoints.MapUserSelfGroup();
            //selfGroup.MapGetMyProfile();
            //selfGroup.MapUpdateMyProfile();
            //selfGroup.MapChangeMyPassword();

            return endpoints;
        }
    }
}
