namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer;
    using EPiServer.Core;
    using EPiServer.Search;
    using EPiServer.Search.Queries;
    using EPiServer.Search.Queries.Lucene;
    using EPiServer.Security;
    using Models;
    using System;
    using System.Linq;
    using System.Web;

    public class SiteSearchService : ISiteSearchService
    {
        private readonly SearchHandler _searchHandler;
        private readonly IContentLoader _contentLoader;

        public SiteSearchService(SearchHandler searchHandler, IContentLoader contentLoader)
        {
            _searchHandler = searchHandler;
            _contentLoader = contentLoader;
        }

        public virtual bool IsActive => SearchSettings.Config.Active;

        public virtual SearchResults Search(HttpContextBase context, SearchServiceFilter filter) =>
            _searchHandler.GetSearchResults(CreateQuery(filter, context), filter.PageNumber, filter.RecordsPerPage);

        protected virtual IQueryExpression CreateQuery(SearchServiceFilter filter, HttpContextBase context)
        {            
            var query = new GroupQuery(LuceneOperator.AND);

            // Term
            query.QueryExpressions.Add(new FuzzyQuery(filter.SearchText, filter.FuzzySimilarityFactor));

            #region Example of custom field search

            //var keywordQuery = new GroupQuery(LuceneOperator.OR);

            //Add free text query to the main query
            //keywordQuery.QueryExpressions.Add(new FuzzyQuery(filter.SearchText, 0.8f));

            // Search in custom field and boost the importance of any hits
            //keywordQuery.QueryExpressions.Add(new CustomFieldQuery(searchText, "WSOL_SEARCH_FIELD", 2.0f));

            #endregion

            string language = filter?.LanguageBranch ?? "en";
            
            //Search for pages using the provided language if specific types aren't set
            if (filter.IncludeContentTypes == null || filter.IncludeContentTypes.Length == 0)
            {
                var pageTypeQuery = new GroupQuery(LuceneOperator.AND);
                pageTypeQuery.QueryExpressions.Add(new ContentQuery<PageData>());
                pageTypeQuery.QueryExpressions.Add(new FieldQuery(language, Field.Culture));

                //Search for media without languages
                var contentTypeQuery = new GroupQuery(LuceneOperator.OR);

                contentTypeQuery.QueryExpressions.Add(new ContentQuery<MediaData>());
                contentTypeQuery.QueryExpressions.Add(pageTypeQuery);

                query.QueryExpressions.Add(contentTypeQuery);
            }
            else
            {
                var contentTypeQueries = new GroupQuery(LuceneOperator.OR);
                query.QueryExpressions.Add(contentTypeQueries);

                foreach (var x in filter.IncludeContentTypes.Select(x => CreateGenericContentQuery(x)).Where(x => x != null))
                {
                    var typeQuery = new GroupQuery(LuceneOperator.AND);
                    typeQuery.QueryExpressions.Add(x);
                    typeQuery.QueryExpressions.Add(new FieldQuery(language, Field.Culture));

                    contentTypeQueries.QueryExpressions.Add(typeQuery);
                }
            }
            
            // Scopes
            if (filter.SearchRoots != null && filter.SearchRoots.Any())
            {
                //Create and add query which groups type conditions using OR
                var scopQueries = new GroupQuery(LuceneOperator.OR);
                query.QueryExpressions.Add(scopQueries);

                foreach (var root in filter.SearchRoots)
                {
                    var contentRootQuery = new VirtualPathQuery();
                    contentRootQuery.AddContentNodes(root, _contentLoader);
                    scopQueries.QueryExpressions.Add(contentRootQuery);
                }
            }

            // Access
            if (filter.FilterPermissions)
            {
                var accessRightsQuery = new AccessControlListQuery();
                accessRightsQuery.AddAclForUser(PrincipalInfo.Current, context);
                query.QueryExpressions.Add(accessRightsQuery);
            }

            if (filter.ExcludeContentTypes == null || filter.ExcludeContentTypes.Length == 0)
                return query;

            // Type Exclusions, original filters must go before excludes
            var excludeCriteria = new GroupQuery(LuceneOperator.NOT);
            excludeCriteria.QueryExpressions.Add(query); 

            foreach (var x in filter.ExcludeContentTypes.Select(x => CreateGenericContentQuery(x)).Where(x => x != null))
                excludeCriteria.QueryExpressions.Add(x);

            return excludeCriteria;
        }

        protected static IQueryExpression CreateGenericContentQuery(Type t)
        {            
            if (typeof(IContentData).IsAssignableFrom(t))
            {
                var type = typeof(ContentQuery<>).MakeGenericType(t);

                return Activator.CreateInstance(type) as IQueryExpression;
            }

            return null;
        }
    }
}