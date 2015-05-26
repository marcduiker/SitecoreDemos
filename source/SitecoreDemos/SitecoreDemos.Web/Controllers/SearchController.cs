using SitecoreDemos.SitecoreLayer.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitecoreDemos.Web.Controllers
{
    public class SearchController : Controller
    {
        private ContentSearch contentSearch;  
        
        public SearchController()
        {
            contentSearch = new ContentSearch();
        }
        
        // GET: Search
        public ActionResult Index(string q)
        {
            var result = contentSearch.Search(q);
            return View(result);
        }
    }
}
