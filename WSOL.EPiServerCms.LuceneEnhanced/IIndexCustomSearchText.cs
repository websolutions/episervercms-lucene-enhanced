namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Core;

    public interface IIndexCustomSearchText
    {
        string GetCustomSearchText(IContent content);
    }
}
