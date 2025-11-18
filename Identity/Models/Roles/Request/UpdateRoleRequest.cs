namespace Identity.Models.Roles.Request
{
    public class UpdateRoleRequest
    {
        /// <summary>
        /// Role name
        /// </summary>
        public required string RoleName { get; init; }
    }
}
