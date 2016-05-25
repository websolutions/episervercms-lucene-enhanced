namespace WSOL.EPiServerCms.LuceneEnhanced.Initialization
{
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.ServiceLocation;

    [InitializableModule]
    public class ContainerInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {            
            context.Container.Configure(x =>
            {
                // singletons
                x.For<IBytesToStringConverter>().Singleton().Use<BytesToStringConverter>();
                x.For<IMediaReader>().Singleton().Use<MediaReader>();
                x.For<ICustomSearchText>().Singleton().Use<CustomSearchText>();
                x.For<IExtendSearchDocument>().Singleton().Use<ExtendSearchDocument>();
                x.For<ISearchAnalyzers>().Singleton().Use<SearchAnalyzers>();

                x.For<IInitializeSearchAnalyzers>().Use<InitializeSearchAnalyzers>();
                x.For<ISiteSearchService>().Use<SiteSearchService>();
            });
        }

        public void Initialize(InitializationEngine context) { }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context) { }

    }
}