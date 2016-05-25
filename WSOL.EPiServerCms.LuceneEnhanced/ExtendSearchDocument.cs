namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Core;
    using Lucene.Net.Documents;

    public class ExtendSearchDocument : IExtendSearchDocument
    {
        public virtual void CustomizeDocument(Document document, IContent content)
        {
            // Example of new field
            //document.Add(new Field("WSOL_SEARCH_FIELD", "bsm", Field.Store.NO, Field.Index.ANALYZED));
        }
    }
}