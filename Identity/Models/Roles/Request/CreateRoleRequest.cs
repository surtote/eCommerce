namespace Identity.Models.Roles.Request
{
    public class CreateRoleRequest
    {
        /// <summary>
        /// Role name
        /// </summary>
        public required string RoleName { get; init; }
    }
}
