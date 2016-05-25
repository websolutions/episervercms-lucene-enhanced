namespace WSOL.EPiServerCms.LuceneEnhanced.Analyzers
{
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using System.IO;

    // TODO: https://lucenenet.apache.org/docs/3.0.3/d0/d92/_standard_analyzer_8cs_source.html, is any of this useful?

    /// <summary>
    /// Tokenizes by whitespace with a lowercase filter.
    /// </summary>
    public class CaseInsensitiveWhitespaceAnalyzer : Analyzer
    {
        /// <summary>
        /// Creates whitespace tokenizer
        /// </summary>
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream t = new WhitespaceTokenizer(reader);
            t = new LowerCaseFilter(t);
            t = new StandardFilter(t);

            return t;
        }
    }
}