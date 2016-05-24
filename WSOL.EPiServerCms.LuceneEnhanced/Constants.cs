using System;
using System.Reflection;

namespace WSOL.EPiServerCms.LuceneEnhanced
{
    public class Constants
    {
        public const string EpiserverDefaultSearchField = "EPISERVER_SEARCH_DEFAULT";

        private static Type _IndexingServiceType = null;

        private static Lucene.Net.Util.Version? _LuceneVersion = null;

        public static Type IndexingServiceType
        {
            get
            {
                if (_IndexingServiceType == null)
                    _IndexingServiceType = Type.GetType("EPiServer.Search.IndexingService.IndexingServiceSettings, EPiServer.Search.IndexingService");

                return _IndexingServiceType;
            }
        }

        public static Lucene.Net.Util.Version LuceneVersion
        {
            get
            {
                if (_LuceneVersion == null)
                    _LuceneVersion = (Lucene.Net.Util.Version)IndexingServiceType.GetProperty("LuceneVersion", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);

                return _LuceneVersion.Value;
            }
        }
    }
}