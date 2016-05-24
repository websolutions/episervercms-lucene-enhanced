using EPiServer.Core;

namespace WSOL.EPiServerCms.LuceneEnhanced
{
    public interface IMediaReader
    {
        byte[] ReadToEnd(MediaData media);
    }
}
