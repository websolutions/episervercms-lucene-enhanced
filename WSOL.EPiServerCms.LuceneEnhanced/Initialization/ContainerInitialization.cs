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
                x.For<IBytesToStringConverter>().Singleton().Use<BytesToStringConverter>();
                x.For<IMediaReader>().Singleton().Use<MediaReader>();
                x.For<ICustomSearchText>().Singleton().Use<CustomSearchText>();
                x.For<ISearchAnalyzers>().Use<SearchAnalyzers>();
                x.For<IInitializeSearchAnalyzers>().Use<InitializeSearchAnalyzers>();
            });
        }

        public void Initialize(InitializationEngine context) { }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context) { }

    }
}