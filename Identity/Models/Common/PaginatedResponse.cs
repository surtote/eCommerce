namespace Identity.Models.Common
{
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// Page data (collection of items)
        /// </summary>
        public required IEnumerable<T> Data { get; init; }

        /// <summary>
        /// Pagination metadata (page info, counts)
        /// </summary>
        public required PaginationMetadata Pagination { get; init; }
    }
}
