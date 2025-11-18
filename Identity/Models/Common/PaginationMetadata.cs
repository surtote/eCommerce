namespace Identity.Models.Common
{
    public class PaginationMetadata
    {
        /// <summary>
        /// Current page number
        /// </summary>
        public required int Page { get; init; }

        /// <summary>
        /// Page size (items per page)
        /// </summary>
        public required int PageSize { get; init; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public required int TotalCount { get; init; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public required int TotalPages { get; init; }
    }
}
