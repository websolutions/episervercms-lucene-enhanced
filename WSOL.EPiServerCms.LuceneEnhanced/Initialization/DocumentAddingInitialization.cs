namespace WSOL.EPiServerCms.LuceneEnhanced.Initialization
{
    using EPiServer.Core;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Logging;
    using EPiServer.Search.IndexingService;
    using EPiServer.ServiceLocation;
    using Extensions;
    using System;

    [ModuleDependency(typeof(ContainerInitialization))]
    public class DocumentAddingInitialization : IInitializableModule
    {
        private readonly ILogger _Logger = LogManager.GetLogger(typeof(DocumentAddingInitialization));

        private static Injected<ICustomSearchText> _CustomSearchText { get; }

        public void Initialize(InitializationEngine context)
        {
            IndexingService.DocumentAdding += IndexingService_DocumentAdding;
        }

        public void Preload(string[] parameters)
        {            
        }

        public void Uninitialize(InitializationEngine context)
        {
            IndexingService.DocumentAdding -= IndexingService_DocumentAdding;
        }

        private void IndexingService_DocumentAdding(object sender, EventArgs e)
        {
            var addUpdateEventArgs = e as AddUpdateEventArgs;

            // Document is not being added/updated
            if (addUpdateEventArgs == null) return;

            // Get the document being indexed
            var document = addUpdateEventArgs.Document;

            // We don't customize VPP file indexing
            if (document.IsUnifiedFileDocument()) return;

            string text = string.Empty;
            var content = document.GetContent<IContent>();

            // TODO: Add hook into add additional fields

            try
            {
                text = _CustomSearchText.Service.GetCustomSearchText(content);
            }
            catch (Exception ex)
            {
                var contentRef = content?.ContentLink?.ID.ToString() ?? "null";
                _Logger.Error($"Failed to parse content ID: {contentRef}!", ex);
            }

            if (string.IsNullOrWhiteSpace(text)) return;

            // Add found data to default field value
            var field = document.GetField(Constants.EpiserverDefaultSearchField);
            field.SetValue($"{field.StringValue} {text}");            

            //document.Add(new Field("WSOL_SEARCH_FIELD", text, Field.Store.NO, Field.Index.ANALYZED));
        }
    }
}