namespace WSOL.EPiServerCms.LuceneEnhanced
{
    using EPiServer.DataAbstraction;
    using EPiServer.PlugIn;
    using EPiServer.Scheduler;
    using EPiServer.Security;
    using System.Security.Principal;
    using System.Web;

    [ScheduledPlugIn(
        DisplayName = "[wsol] Re-Index Site Content",
        Description = "This tool will go through all the content and files on the site and add them to the search index.",
        SortIndex = 9999,
        DefaultEnabled = true,
        InitialTime = "4.0:0:0",
        IntervalLength = 1440,
        IntervalType = ScheduledIntervalType.Months)]
    public class ReIndexContentScheduledJob : ScheduledJobBase
    {
        public override string Execute()
        {
            if (HttpContext.Current == null)
            {
                PrincipalInfo.CurrentPrincipal = new GenericPrincipal
                (
                    new GenericIdentity("Scheduled DummyTask"),
                    new[] { "Administrators" }
                );
            }

            var data = new Controllers.SearchIndexerController().ReIndexContent(true);

            return $"Latest complete indexing: {data.IndexInformation.ExecutionDate}";
        }
    }
}