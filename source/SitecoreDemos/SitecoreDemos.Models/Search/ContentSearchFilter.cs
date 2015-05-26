using System.Collections.Generic;

namespace SitecoreDemos.Models.Search
{
    public class ContentSearchFilter
    {
        public ContentSearchFilter()
        {
            Templates = new List<string>();
        }
        
        public string SearchText { get; set; }

        public List<string> Templates { get; set; }

        public Paging Paging { get; set; }
    }
}
