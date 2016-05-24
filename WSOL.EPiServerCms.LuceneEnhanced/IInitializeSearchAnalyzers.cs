namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using System;

    public interface IInitializeSearchAnalyzers
    {
        void Initialize(Type indexingServiceSettingsType, Lucene.Net.Util.Version luceneVersion, ISearchAnalyzers analzyers);
    }
}
