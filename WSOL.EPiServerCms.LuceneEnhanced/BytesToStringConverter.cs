namespace WSOL.EPiServerCms.LuceneEnhanced
{
    /// <summary>
    /// Default service does nothing, needs extra packages
    /// </summary>
    public class BytesToStringConverter : IBytesToStringConverter
    {
        public virtual string ConvertToString(byte[] bytes) => null;
    }
}