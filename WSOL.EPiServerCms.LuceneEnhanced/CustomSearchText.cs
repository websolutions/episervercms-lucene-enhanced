﻿namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Core;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Default service only reads blobs from MediaData object, can be customized for other types as well
    /// </summary>
    public class CustomSearchText : ICustomSearchText
    {
        protected static Injected<IMediaReader> _MediaReader { get; set; }

        protected static Injected<IBytesToStringConverter> _ByteConverter { get; set; }

        /// <summary>
        /// Default only reads blob data when a custom IBytesToStringConverter is provided to the container. Override in container to read in additional data.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual string GetCustomSearchText(IContent content)
        {
            MediaData searchableDoc = content as MediaData;
            
            if (searchableDoc?.BinaryData != null)
            {
                byte[] bytes = _MediaReader.Service.ReadToEnd(searchableDoc);

                if (bytes != null)
                {
                    return _ByteConverter.Service.ConvertToString(bytes)?.Trim();
                }
            }

            return null;
        }
    }
}