namespace WSOL.EPiServerCms.LuceneEnhanced.Initialization
{
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.ServiceLocation;
    using System;

    [ModuleDependency(typeof(ContainerInitialization))]
    public class SearchServiceInitialization : IInitializableModule
    {
        private static Injected<ISearchAnalyzers> _Analyzers { get; }

        private static Injected<IInitializeSearchAnalyzers> _InitializeService { get; }

        public void Initialize(InitializationEngine context)
        {
            context.InitComplete += Context_InitComplete;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            context.InitComplete -= Context_InitComplete;
        }

        private void Context_InitComplete(object sender, EventArgs e)
        {
            _InitializeService.Service.Initialize(Constants.IndexingServiceType, Constants.LuceneVersion, _Analyzers.Service);
        }
    }
}