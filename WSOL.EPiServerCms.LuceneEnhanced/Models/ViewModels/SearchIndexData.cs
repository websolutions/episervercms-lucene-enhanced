namespace WSOL.EPiServerCms.LuceneEnhanced.Models.ViewModels
{
    using EPiServer.Core;

    public class SearchIndexData
    {
        public bool Posted { get; set; }

        public IndexingInformation IndexInformation { get; set; }
    }
}