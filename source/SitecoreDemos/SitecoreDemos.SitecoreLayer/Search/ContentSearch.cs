using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Links;
using SitecoreDemos.Models.Search;
using SitecoreDemos.SitecoreLayer.Templates;
using SitecoreSolution.SitecoreLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SitecoreDemos.SitecoreLayer.Search
{
    public class ContentSearch
    {
        /// <summary>
        /// ContentSearch based on a seach string.
        /// </summary>
        /// <param name="searchstring">The searchstring.</param>
        /// <returns>A collection of ContentSearchResultItem limited to 5 items.</returns>
        public ContentSearchResult Search(string searchstring)
        {
            const int resultLimit = 5;

            var result = new ContentSearchResult();
            string index = GetSearchIndexName();

            using (var context = ContentSearchManager.GetIndex(index).CreateSearchContext())
            {
                var query = context.GetQueryable<SearchResultItem>();
                ApplySearchStringQuery(searchstring, ref query);

                // Result is already ranked based on score.
                var searchResult = query.GetResults().Hits.Take(resultLimit).ToList();

                UpdateResultWithItem(
                    result,
                    searchResult,
                    NewsTemplate.ID);
                UpdateResultWithItem(
                    result,
                    searchResult,
                    EventTemplate.ID);

                result.ResultCount = result.ContentSearchResultItems.Count;

                return result;
            }
        }

        private static string GetSearchIndexName()
        {
            return string.Format(
                "custom_search_{0}_index",
                Sitecore.Context.Database.Name.ToLower());
        }

        /// <summary>
        /// ContentSearch based on search text and templates (facets).
        /// </summary>
        /// <param name="filter">The ContentSearchFilter.</param>
        /// <returns></returns>
        public ContentSearchResult SearchWithFacets(ContentSearchFilter filter)
        {
            var contentSearchResult = new ContentSearchResult
            {
                Paging = new Paging(),
                ContentSearchResultItems = new List<ContentSearchResultItem>()
            };
            string index = GetSearchIndexName();

            using (var context = ContentSearchManager.GetIndex(index).CreateSearchContext())
            {
                
                // Null query, will be updated with search text and template filter.
                var query = context.GetQueryable<SearchResultItem>();

                // Retrieve all possible facets before a query is done with the search text.
                var facetResults = query.FacetOn(item => item.TemplateId).GetFacets();
                FacetResults facetResultsAfterSearch = null;
                if (filter != null)
                {
                    ApplySearchStringQuery(filter.SearchText, ref query);
                    facetResultsAfterSearch = query.FacetOn(item => item.TemplateId).GetFacets();
                    ApplyTemplateQuery(filter.Templates, ref query);

                }
                var searchResult = query.FacetOn(item => item.TemplateId).GetResults();
                UpdateResultWithFacets(facetResults, facetResultsAfterSearch, searchResult, contentSearchResult);

                var distinctHits = searchResult.Hits.Distinct().ToList();

                UpdateResultWithItem(
                    contentSearchResult,
                    distinctHits,
                    NewsTemplate.ID);
                UpdateResultWithItem(
                    contentSearchResult,
                    distinctHits,
                    EventTemplate.ID);


                contentSearchResult.ResultCount = contentSearchResult.ContentSearchResultItems.Count;

                int numberToSkip;
                UpdateResultWithPaging(contentSearchResult, filter, out numberToSkip);
                contentSearchResult.ContentSearchResultItems = contentSearchResult.ContentSearchResultItems.Skip(numberToSkip).Take(contentSearchResult.Paging.PageSize).ToList();

                return contentSearchResult;
            }
        }

        public ContentSearchFilter GetContentSearchFilter(string searchText, string templateId)
        {
            return new ContentSearchFilter
            {
                SearchText = searchText,
                Paging = new Paging(),
                Templates = new List<string>{templateId}
            };
        }

        internal static void UpdateResultWithPaging(ContentSearchResult result, ContentSearchFilter filter, out int numberToSkip)
        {
            result.Paging.PageSize = filter.Paging.PageSize;
            result.Paging.CurrentPage = filter.Paging.CurrentPage == 0 ? 1 : filter.Paging.CurrentPage;
            result.Paging.TotalPageCount = (int)Math.Ceiling((double)result.ContentSearchResultItems.Count / result.Paging.PageSize);
            numberToSkip = result.Paging.CurrentPage == 1 ? 0 : (result.Paging.CurrentPage - 1) * result.Paging.PageSize;
        }

        /// <summary>
        /// Updates the content search result with facets.
        /// </summary>
        /// <param name="facetResults">All possible facets.</param>
        /// <param name="facetResultsAfterSearch">Facets that contain results after querying using the search text. This is used to get the result count per facet.</param>
        /// <param name="searchResults">The collection of SearchResultItems after the search on searchtext and filtering on templates.</param>
        /// <param name="contentSearchResult">Result object which is updated with facets.</param>
        private void UpdateResultWithFacets(FacetResults facetResults, FacetResults facetResultsAfterSearch, SearchResults<SearchResultItem> searchResults, ContentSearchResult contentSearchResult)
        {
            var allFacets = facetResults.Categories.FirstOrDefault();
            FacetCategory searchFacets = null;
            if (facetResultsAfterSearch != null)
            {
                searchFacets = facetResultsAfterSearch.Categories.FirstOrDefault();
            }
            var searchResultFacets = searchResults.Facets.Categories.FirstOrDefault();
            if (allFacets != null && searchFacets != null)
            {
                contentSearchResult.Facets = allFacets.Values.Select(facet => GetFacet(facet, searchFacets, searchResultFacets)).OrderBy(facet => facet.Name).ToList();
            }
        }

        private void ApplySearchStringQuery(string searchstring, ref IQueryable<SearchResultItem> query)
        {
            if (!string.IsNullOrEmpty(searchstring))
            {
                var predicate = PredicateBuilder.True<SearchResultItem>();
                // If there are multiple search terms a search will be done for each term.
                var searchTerms = searchstring.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // Search in the specific fields of the templates instead of the computed Content field.
                predicate = searchTerms.Aggregate(predicate, (current, term) => current.Or(
                    item => item[PublicationBaseTemplate.Fields.Text].Contains(term) ||
                            item[PublicationBaseTemplate.Fields.Title].Contains(term) ||
                            item[EventTemplate.Fields.Location].Contains(term)));

                query = query.Filter(predicate);
            }
        }

        private static void ApplyTemplateQuery(IList<string> templates, ref IQueryable<SearchResultItem> query)
        {
            if (templates != null && templates.Any() && templates.All(t => !string.IsNullOrEmpty(t)))
            {
                var templateIdCollection = new List<ID>();
                foreach (var template in templates)
                {
                    ID id;
                    if (TryGetSitecoreId(template, out id))
                    {
                        templateIdCollection.Add(id);
                    }
                }

                query = query.Where(result => templateIdCollection.Contains(result.TemplateId));
            }
        }

        private static void UpdateResultWithItem(
            ContentSearchResult contentSearchResult,
            IEnumerable<SearchHit<SearchResultItem>> searchHitCollection,
            ID templateId)
        {
            var searchItems = searchHitCollection.Where(
                    result => result.Document.TemplateId == templateId);

            foreach (var searchItem in searchItems)
            {
                var item = ItemHelper.GetItem(searchItem.Document.ItemId);
                if (item != null)
                {
                    var resultaat = new ContentSearchResultItem
                    {
                        Title = item[PublicationBaseTemplate.Fields.Title],
                        Url = LinkManager.GetItemUrl(item)
                    };

                    contentSearchResult.ContentSearchResultItems.Add(resultaat);
                }
            }
        }

        /// <summary>
        /// Gets the facet.
        /// </summary>
        /// <param name="facetValue">The name of the facet (Sitecore template ID).</param>
        /// <param name="searchFacetCategory">The FacetCategory after searching on the searchtext. This contains the resultcount per facet.</param>
        /// <param name="searchResultsCategory">The FacetCategory after filtering on template. This is used to determine which facets are selected.</param>
        /// <returns></returns>
        internal ContentSearchFacet GetFacet(FacetValue facetValue, FacetCategory searchFacetCategory, FacetCategory searchResultsCategory)
        {
            // A FacetValue doesn't contain a usable description of the facet, just the template ID (as a string).
            // A lookup will be done to get a usable desciption for the facet.

            var templateId = facetValue.Name;
            var searchFacetValue = searchFacetCategory.Values.FirstOrDefault(f => f.Name == facetValue.Name);
            var searchResultFacetValue = searchResultsCategory.Values.FirstOrDefault(f => f.Name == facetValue.Name);

            var facet = new ContentSearchFacet
            {
                ResultCount = searchFacetValue != null ? searchFacetValue.AggregateCount : 0,
                IsSelected = searchResultFacetValue != null,
                TemplateId = templateId,
                Name = GetFacetName(templateId)
            };

            return facet;
        }

        internal string GetFacetName(string templateId)
        {
            ID id;
            if (TryGetSitecoreId(templateId, out id))
            {
                return ItemHelper.GetItem(id).DisplayName;
            }

            return string.Empty;
        }

        internal static bool TryGetSitecoreId(string itemId, out ID id)
        {
            var guid = new Guid(itemId);

            return ID.TryParse(guid, out id);
        }
    }
}
