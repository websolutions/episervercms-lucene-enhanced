namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Core;
    using EPiServer.Search;
    using EPiServer.ServiceLocation;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Default service only reads blobs from MediaData object, can be customized for other types as well
    /// </summary>
    public class IndexCustomSearchText : IIndexCustomSearchText
    {
        protected static Injected<IMediaReader> _MediaReader { get; set; }

        protected static Injected<IBytesToStringConverter> _ByteConverter { get; set; }

        /// <summary>
        /// Default reads blob data when a custom IBytesToStringConverter is provided to the container, or IContent models that implement ICustomSearchText.
        /// Override in container to read in additional data.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual string GetCustomSearchText(IContent content)
        {
            ISearchable searchableContent = content as ISearchable;
            MediaData searchableDoc = content as MediaData;
            string customText = null;
            
            if (searchableDoc?.BinaryData != null)
            {
                byte[] bytes = _MediaReader.Service.ReadToEnd(searchableDoc);

                if (bytes != null)
                {
                    customText = _ByteConverter.Service.ConvertToString(bytes, searchableDoc.MimeType)?.Trim();

                    if (!string.IsNullOrWhiteSpace(customText))
                        return Regex.Replace(customText, @"<(.|\n)*?>", string.Empty);
                }
            }


            ICustomSearchText customSearchText = content as ICustomSearchText;

            if(customSearchText != null)
            {
                string t = customSearchText.CustomSearchText;

                if (!string.IsNullOrWhiteSpace(t))
                {
                    customText = t;

                    if (!string.IsNullOrWhiteSpace(customText))
                        return Regex.Replace(customText, @"<(.|\n)*?>", string.Empty);
                }
            }

            return null;
        }
    }
}