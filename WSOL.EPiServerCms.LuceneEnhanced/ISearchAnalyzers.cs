namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using Lucene.Net.Analysis;

    public interface ISearchAnalyzers
    {
        Analyzer DefaultAnalyzer { get; }

        Analyzer TextAnalyzer { get; }        
    }
}
