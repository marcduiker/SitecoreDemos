using System.Collections.Generic;

namespace SitecoreDemos.Models.Search
{
    public class ContentSearchResult
    {
        public ContentSearchResult()
        {
            ContentSearchResultItems = new List<ContentSearchResultItem>();
            Facets = new List<ContentSearchFacet>();
        }
        
        public List<ContentSearchResultItem> ContentSearchResultItems { get; set; }

        public Paging Paging { get; set; }

        public List<ContentSearchFacet> Facets { get; set; }

        public int ResultCount { get; set; }
    }
}
