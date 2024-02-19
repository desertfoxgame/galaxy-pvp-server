namespace GalaxyPvP.Data.Model
{
    public class PageResponse<T>
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPrev {  get; set; }

        public bool HasNext { get; set; }

        public List<T> Data { get; set; } = new List<T>();

    }
}
