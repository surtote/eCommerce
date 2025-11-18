namespace Identity.Models.Common
{
    public class ErrorResponse
    {
        /// <summary>
        /// Collection of items for the current page
        /// </summary>
        /// <summary>
        /// List of error messages
        /// </summary>
        public required IEnumerable<string> Errors { get; init; }

        /// <summary>
        /// General error message (optional)
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// HTTP status code (optional)
        /// </summary>
        public int? StatusCode { get; init; }
    }
}