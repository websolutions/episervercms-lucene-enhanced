namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Core;

    public interface ICustomSearchText
    {
        string GetCustomSearchText(IContent content);
    }
}
