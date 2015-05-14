using System.Collections.Generic;

namespace SitecoreDemos.Models.Search
{
    public class ContentSearchResult
    {
        public List<ContentSearchResultItem> ContentSearchResultItems { get; set; }

        public Paging Paging { get; set; }

        public List<ContentSearchFacet> Facets { get; set; }

        public int ResultCount { get; set; }
    }
}
