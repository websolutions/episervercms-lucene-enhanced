namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.Search.Queries;
    using EPiServer.Search.Queries.Lucene;
    using System.Globalization;

    public class CustomFieldQuery : IQueryExpression
    {
        public CustomFieldQuery(string queryExpression, string fieldName)
        {
            Expression = queryExpression;
            Field = fieldName;
            Boost = null;
        }

        public CustomFieldQuery(string queryExpression, string fieldName, float boost)
        {
            Expression = queryExpression;
            Field = fieldName;
            Boost = boost;
        }

        public string GetQueryExpression() =>
            string.Format("{0}:({1}{2})",
                Field,
                LuceneHelpers.EscapeParenthesis(Expression),
                Boost.HasValue ? string.Concat("^", Boost.Value.ToString(CultureInfo.InvariantCulture).Replace(",", ".")) : string.Empty);

        public string Field { get; set; }

        public string Expression { get; set; }

        public float? Boost { get; set; }
    }
}