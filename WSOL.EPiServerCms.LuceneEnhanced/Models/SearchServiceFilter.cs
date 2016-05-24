namespace WSOL.EPiServerCms.LuceneEnhanced.Models
{
    using EPiServer.Core;
    using EPiServer.Web;
    using System;
    using System.Collections.Generic;

    public class SearchServiceFilter
    {
        public virtual Type[] ExcludeContentTypes { get; set; }

        public virtual bool FilterPermissions { get; set; } = true;

        public virtual float FuzzySimilarityFactor { get; set; } = 0.8f;

        public virtual Type[] IncludeContentTypes { get; set; }

        public virtual string LanguageBranch { get; set; }

        public virtual int PageNumber { get; set; } = 1;

        public virtual int RecordsPerPage { get; set; } = 10;

        public virtual IEnumerable<ContentReference> SearchRoots { get; set; } =
            new[] { SiteDefinition.Current.StartPage, SiteDefinition.Current.GlobalAssetsRoot, SiteDefinition.Current.SiteAssetsRoot };

        public virtual string SearchText { get; set; }
    }
}