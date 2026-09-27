namespace Application.Dtos
{
    public class PagedResultDto<T>
    {
        public IReadOnlyCollection<T> Items { get; set; } = new List<T>().AsReadOnly();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
