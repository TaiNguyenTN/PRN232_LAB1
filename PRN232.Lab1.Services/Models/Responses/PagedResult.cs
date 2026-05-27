namespace PRN232.Lab1.Services.Models.Responses
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PaginationMeta Pagination { get; set; }
    }

    public class PaginationMeta
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
