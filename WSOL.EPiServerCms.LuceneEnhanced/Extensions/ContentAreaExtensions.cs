﻿namespace WSOL.EPiServerCms.LuceneEnhanced.Extensions
{
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.ServiceLocation;
    using EPiServer.SpecializedProperties;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ContentAreaExtensions
    {
        public static string GetContentAreaContents(this ContentArea contentArea)
        {
            if (contentArea == null) { return string.Empty; }
            StringBuilder stringBuilder = new StringBuilder();

            foreach (ContentAreaItem contentAreaItem in contentArea.Items)
            {
                IContent blockData = contentAreaItem.GetContent();
                IEnumerable<string> props = GetSearchablePropertyValues(blockData, blockData.ContentTypeID);
                stringBuilder.AppendFormat(" {0}", string.Join(" ", props));
            }

            return stringBuilder.ToString();
        }

        public static IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, ContentType contentType)
        {
            if (contentType == null)
            {
                yield break;
            }

            foreach (PropertyDefinition current in from d in contentType.PropertyDefinitions where 
                                                   d.Searchable || typeof(IPropertyBlock).IsAssignableFrom(d.Type.DefinitionType)
                                                   select d)
            {
                PropertyData propertyData = contentData.Property[current.Name];
                IPropertyBlock propertyBlock = propertyData as IPropertyBlock;

                if (propertyBlock != null)
                {
                    foreach (string current2 in GetSearchablePropertyValues(propertyBlock.Block, propertyBlock.BlockPropertyDefinitionTypeID))
                    {
                        yield return current2;
                    }
                }
                else
                {
                    yield return propertyData.ToWebString();
                }
            }

            yield break;
        }

        public static IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, int contentTypeID) =>
            GetSearchablePropertyValues(contentData, ServiceLocator.Current.GetInstance<IContentTypeRepository>().Load(contentTypeID));
    }
}