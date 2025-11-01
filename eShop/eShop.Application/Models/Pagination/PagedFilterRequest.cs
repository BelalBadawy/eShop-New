namespace eShop.Application.Models.Pagination
{
    public class PagedFilterRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }          // optional search filter
        public string? SortBy { get; set; } = "UserName"; // field name
        public string? SortDirection { get; set; } = "asc"; // asc or desc
    }
}
