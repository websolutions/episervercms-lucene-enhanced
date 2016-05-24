using System;
using Lucene.Net.Analysis;
using System.Reflection;

namespace WSOL.EPiServerCms.LuceneEnhanced
{
    public class InitializeSearchAnalyzers : IInitializeSearchAnalyzers
    {
        /// <summary>
        /// Initalize all analyzers for search service
        /// </summary>
        /// <param name="indexingServiceSettingsType"></param>
        /// <param name="luceneVersion"></param>
        public virtual void Initialize(Type indexingServiceSettingsType, Lucene.Net.Util.Version luceneVersion, ISearchAnalyzers analyzers)
        {
            var defaultAnalyzer = analyzers.DefaultAnalyzer;
            var textAnalyzer = analyzers.TextAnalyzer;
            
            PerFieldAnalyzerWrapper fieldAnalyzerWrapper = new PerFieldAnalyzerWrapper(defaultAnalyzer);
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_ID", new KeywordAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_CULTURE", new KeywordAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_REFERENCEID", new KeywordAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_AUTHORSTORAGE", new KeywordAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_CATEGORIES", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_ACL", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_VIRTUALPATH", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_TYPE", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_CREATED", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_MODIFIED", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_PUBLICATIONEND", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_PUBLICATIONSTART", new WhitespaceAnalyzer());
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_ITEMSTATUS", new WhitespaceAnalyzer());
            
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_TITLE", textAnalyzer);
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_DISPLAYTEXT", textAnalyzer);
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_AUTHORS", textAnalyzer);
            fieldAnalyzerWrapper.AddAnalyzer("EPISERVER_SEARCH_DEFAULT", textAnalyzer);

            // Replace default analyzer
            indexingServiceSettingsType
                .GetField("_analyzer", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, fieldAnalyzerWrapper);
        }
    }
}