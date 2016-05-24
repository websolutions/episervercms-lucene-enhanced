namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;

    /// <summary>
    /// Provides search analyzers to search service intialization
    /// </summary>
    public class SearchAnalyzers : ISearchAnalyzers
    {
        /// <summary>
        /// Default content analyzer found from DLL
        /// </summary>
        public virtual Analyzer DefaultAnalyzer => new StandardAnalyzer(Constants.LuceneVersion, StopFilter.MakeStopSet(new string[0])); 

        /// <summary>
        /// Custom text analyzer to support hyphenated words
        /// </summary>
        public virtual Analyzer TextAnalyzer => new Analyzers.CaseInsensitiveWhitespaceAnalyzer();
    }
}