namespace WSOL.EPiServerCms.LuceneEnhanced.Initialization
{
    using EPiServer;
    using EPiServer.Core;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Search;
    using System.Globalization;
    
    /// <summary>
    /// Removes ISearchable items on saving and publishing events
    /// </summary>
    [InitializableModule]
    public class ClearSearchItemInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.SavingContent += ClearSearchItem;
            DataFactory.Instance.PublishingContent += ClearSearchItem;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.SavingContent -= ClearSearchItem;
            DataFactory.Instance.PublishingContent -= ClearSearchItem;
        }

        private void ClearSearchItem(object sender, ContentEventArgs e)
        {
            ISearchable seachable = e.Content as ISearchable;

            if (seachable?.IsSearchable == true) return;

            CultureInfo language = null;
            ILocalizable localizable = e.Content as ILocalizable;

            if (localizable != null)
                language = localizable.Language;

            string searchId = string.Concat(e.Content.ContentGuid, "|", language);
            IndexRequestItem indexItem = new IndexRequestItem(searchId, IndexAction.Remove);

            SearchHandler.Instance.UpdateIndex(indexItem);
        }
    }
}