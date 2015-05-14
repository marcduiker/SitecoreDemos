namespace SitecoreDemos.Models.Search
{
    public class Paging
    {
        public Paging()
        {
            // Set a default page size in case the property is not set.
            PageSize = 10;
        }
        
        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPageCount { get; set; }
    }
}
