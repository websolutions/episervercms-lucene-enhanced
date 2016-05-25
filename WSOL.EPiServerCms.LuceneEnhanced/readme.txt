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
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.ServiceLocation;
    using System.Text;
    using WSOL.EPiServerCms.LuceneEnhanced;
    using WSOL.EPiServerCms.LuceneEnhanced.Initialization;

    [ModuleDependency(typeof(ContainerInitialization))]
    public class SearchMediaDataIndexer : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(x =>
            {
                // Use a NuGet package: like iTextSharp or TikaOnDotnet.TextExtractor to extract PDF,Doc(x), PPT(X), and XLS(X) documents
                x.For<IBytesToStringConverter>().Singleton().Use<CustomBytesToStringConverter>();

                // To add custom search text add the ICustomSearchText inteface to your models and return the indexable text.

                // To extend indexed fields implement and register a class that inherits IExtendSearchDocument and do the following in the CustomizeDocument method:
                //document.Add(new Field("Name_Of_Field", "TEXT_VALUE_TO_STORE", Field.Store.NO, Field.Index.ANALYZED));
            });
        }

        public void Preload(string[] parameters) { }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }
    }

    /// <summary>
    /// Requires a NuGet package iTextSharp or TikaOnDotnet.TextExtractor to extract PDF,Doc(x), PPT(X), and XLS(X) documents
    /// </summary>
    internal class CustomBytesToStringConverter : BytesToStringConverter
    {
        public override string ConvertToString(byte[] bytes, string mimeType)
        {
            // Using itextsharp
            var mType = mimeType.ToLowerInvariant();

            if (mType == "application/pdf")
            {
                using (var reader = new iTextSharp.text.pdf.PdfReader(bytes))
                {
                    StringBuilder s = new StringBuilder();
                    var parserStrategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    var totalPages = reader.NumberOfPages;

                    for (int i = 1; i <= totalPages; i++)
                    {
                        s.AppendFormat(" {0}", iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i, parserStrategy));
                    }

                    return s.ToString().Trim();
                }
            }

            // using TikaOnDotNet.TextExtraction
            //return new TikaOnDotNet.TextExtraction.TextExtractor().Extract(bytes).Text?.Trim();

            return null;
        }

    }
}
```