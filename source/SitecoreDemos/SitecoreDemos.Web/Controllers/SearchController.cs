using SitecoreDemos.SitecoreLayer.Search;
using System.Web.Mvc;

namespace SitecoreDemos.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ContentSearch contentSearch;  
        
        public SearchController()
        {
            contentSearch = new ContentSearch();
        }
        
        // GET: Search
        public ActionResult Search(string q)
        {
            var result = contentSearch.Search(q);
            return View(result);
        }

        // GET: Faceted Search
        public ActionResult FacetedSearch(string q, string t)
        {
            var filter = contentSearch.GetContentSearchFilter(q, t);
            var result = contentSearch.SearchWithFacets(filter);
            return View(result);
        }
    }
}
