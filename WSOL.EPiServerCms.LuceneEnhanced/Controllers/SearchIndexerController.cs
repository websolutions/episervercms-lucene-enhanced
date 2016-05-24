namespace WSOL.EPiServerCms.LuceneEnhanced.Controllers
{
    using EPiServer.Core;
    using EPiServer.Data.Dynamic;
    using EPiServer.Logging;
    using EPiServer.Search;
    using EPiServer.ServiceLocation;
    using EPiServer.Shell.Gadgets;
    using Models.ViewModels;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;

    [Gadget(Name = "ReIndex Content", Title = "ReIndex Content")]
    [Authorize(Roles = "CommerceAdmins, WebAdmins, Administrators")]
    //[EPiServer.Shell.Web.ScriptResource("Scripts/StoreExport.js")]
    public class SearchIndexerController : Controller
    {
        private static readonly ILogger _Logger = LogManager.GetLogger(typeof(SearchIndexerController));
        
        public ActionResult Index() => View(GetModel());

        [HttpPost]
        public ActionResult Index(string resetIndex)
        {
            var model = GetModel(true);
            
            if (SearchSettings.Config.Active)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReIndex), resetIndex == "reset");
            }

            return View(model);
        }

        private SearchIndexData GetModel(bool posted = false)
        {
            var model = new SearchIndexData();
            model.Posted = posted;
            model.IndexInformation = Enumerable.FirstOrDefault(Store.LoadAll<IndexingInformation>()) ?? new IndexingInformation();

            return model;
        }
        
        private DynamicDataStore Store =>
            DynamicDataStoreFactory.Instance.GetStore(typeof(IndexingInformation)) ??
            DynamicDataStoreFactory.Instance.CreateStore(typeof(IndexingInformation));

        private void ReIndex(object state)
        {
            try
            {                
                bool reset = (state as bool?) ?? false;

                if (reset)
                    ServiceLocator.Current.GetInstance<ReIndexManager>().ReIndex();

                SaveIndexingInformation(reset);
            }
            catch (Exception ex)
            {
                _Logger.Error("Failed to reset index!", ex);
            }
        }

        private void SaveIndexingInformation(bool reset)
        {
            IndexingInformation indexingInformation = Enumerable.FirstOrDefault(Store.LoadAll<IndexingInformation>()) ?? new IndexingInformation();
            indexingInformation.ExecutionDate = DateTime.Now;
            indexingInformation.ResetIndex = reset;
            Store.Save(indexingInformation);
        }
    }
}