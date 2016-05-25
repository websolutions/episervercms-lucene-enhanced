namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Core;
    using Lucene.Net.Documents;

    public interface IExtendSearchDocument
    {
        void CustomizeDocument(Document document, IContent content);
    }
}
