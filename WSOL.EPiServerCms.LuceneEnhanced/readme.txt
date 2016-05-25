# WSOL.EPiServerCms.LuceneEnhanced

## Important

For MediaData types to get indexed please use the SearchDocumentIndexer InitializationModule example in this document

Includes an initialization module to remove content models that implement the ISearchable module and return false.

A scheduled job [wsol]Re-Index Site Content, has been included, please configure a desired interval or disable as this job always deletes the current index!

## Noteable extensibile areas

* IBytesToStringConverter: used to convert MediaData objects to their string contents, example provided below in SearchDocumentIndexer
* ICustomSearchText: can be applied to any IContent model that provides an additional string of indexable data, for instance ContentArea properties
* IExtendSearchDocument: allows creation of custom lucene fields to index
* ISiteSearchService: change default site search behaviour, or simple create your own class for your search page controller
* ISearchAnalyzers: change default index analyzers, for example the default text analyzer of this package is a whitespace tokenizer to allow hyphenated words.
* IInitializeSearchAnalyzers: change how defaut fields are analyzed.

## SearchDocumentIndexer Code Example of creating class to index pdf/word/ppt/xls documents

```
namespace Example
{
    using EPiServer.ServiceLocation;
    using EPiServer.Framework.Initialization;
    using EPiServer.Framework;
    using WSOL.EPiServerCms.LuceneEnhanced;
    using WSOL.EPiServerCms.LuceneEnhanced.Initialization;

    [ModuleDependency(typeof(ContainerInitialization))]
    public class SearchMediaDataIndexer : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(x =>
            {
                // Use the NuGet package: TikaOnDotnet.TextExtractor to extract PDF,Doc(x), PPT(X), and XLS(X) documents
                x.For<IBytesToStringConverter>().Singleton().Use<CustomBytesToStringConverter>();

                // To add custom search text add the ICustomSearchText inteface to your models and return the indexable text.

                // To extend indexed fields implement and register a class that inherits IExtendSearchDocument and do the following:
                //document.Add(new Field("Name_Of_Field", "TEXT_VALUE_TO_STORE", Field.Store.NO, Field.Index.ANALYZED));
            });
        }

        public void Preload(string[] parameters) { }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }
    }

    /// <summary>
    /// Requires NuGet Package TikaOnDotnet.TextExtractor to extract PDF,Doc(x), PPT(X), and XLS(X) documents
    /// </summary>
    internal class CustomBytesToStringConverter : BytesToStringConverter
    {
        public override string ConvertToString(byte[] bytes, string mimeType) =>
            new TikaOnDotNet.TextExtraction.TextExtractor().Extract(bytes).Text?.Trim();
    }
}
```