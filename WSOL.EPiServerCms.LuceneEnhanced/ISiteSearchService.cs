namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Search;
    using Models;
    using System.Web;

    public interface ISiteSearchService
    {
        bool IsActive { get; }

        SearchResults Search(HttpContextBase context, SearchServiceFilter filter);
    }
}
