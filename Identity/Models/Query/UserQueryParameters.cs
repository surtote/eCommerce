using Microsoft.AspNetCore.Mvc;

namespace Identity.Models.Query
{
    public record UserQueryParameters(
        [FromQuery] int Page = 1,
        [FromQuery] int PageSize = 10,
        [FromQuery] string? Search = null,
        [FromQuery] string? Role = null,
        [FromQuery] string? SortBy = null,
        [FromQuery] bool? SortDescending = null)
    {
        /// <summary>
        /// Page number (starts at 1)
        /// </summary>
        public int Page { get; } = Page;

        /// <summary>
        /// Page size (items per page, max 100)
        /// </summary>
        public int PageSize { get; } = PageSize;

        /// <summary>
        /// Search term (searches email and username, case-insensitive)
        /// </summary>
        public string? Search { get; } = Search;

        /// <summary>
        /// Filter by role name
        /// </summary>
        public string? Role { get; } = Role;

        /// <summary>
        /// Sort field (email, userName). Optional - defaults to email if not specified.
        /// </summary>
        public string? SortBy { get; } = SortBy;

        /// <summary>
        /// Sort in descending order. Optional - defaults to false (ascending) if not specified.
        /// </summary>
        public bool? SortDescending { get; } = SortDescending;
    }
}