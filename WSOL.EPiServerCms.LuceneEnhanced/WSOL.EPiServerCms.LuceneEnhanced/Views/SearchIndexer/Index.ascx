<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<WSOL.EPiServerCms.LuceneEnhanced.Models.ViewModels.SearchIndexData>" %>
<%@ Import Namespace="EPiServer.Shell.Web.Mvc.Html" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Web.Mvc" %>

<div class="epi-padding-small wsol-search-indexer">
    <% if (Model.Posted)
        { %>
        <div class="posted-message">ReIndex has been queued!</div>
    <% } %>
    <% using (Html.BeginGadgetForm("Index", "SearchIndexer"))
       { %>
    <div class="editor-field">
        <label for="resetIndex">Delete old data:</label>
        <input type="checkbox" name="resetIndex" id="resetIndex" value="reset" <%: Model.IndexInformation.ResetIndex ? "checked" : "" %> />
        <button type="submit">Start Indexing</button>
    </div>
    <% } %>    
    <div class="information">
        Latest complete indexing: <%= Model.IndexInformation.ExecutionDate %>
    </div>    
</div>