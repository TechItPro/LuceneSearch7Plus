using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace Search.UI.LuceneSearch
{
    public partial class LuceneSearchResults : System.Web.UI.UserControl
    {
        private string baseURL;
        private string searchKeyword;
        public string IndexName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            int pageSize = 10; //default page size
            searchKeyword = Server.UrlDecode(WebUtil.GetQueryString("searchStr"));
            if (!IsPostBack)
            {
                txtKeywords.Text = searchKeyword;
                if (int.TryParse(CommonText.get("Search Result Page Size"), out pageSize))
                {
                    gvSearchResults.PageSize = pageSize;
                }
                GetSearchResults(searchKeyword);
            }
        }

        private void GetSearchResults(string searchKeyword)
        {
            string indexName = StringUtil.GetString(IndexName, CommonText.get("Search Index"));


            // Decode the search string query string
            string searchStr = searchKeyword;

            // Category is provided if a visitor wants to see all the results for a given area of the site
            string categoryStr = Server.UrlDecode(WebUtil.GetQueryString("categoryStr"));
            baseURL = string.Format("{0}?searchStr={1}&categoryStr=", LinkManager.GetItemUrl(Sitecore.Context.Item), searchStr);

            // Be sure we have an empty search string at very least (avoid null)
            if (searchKeyword == null) searchKeyword = string.Empty;

            // If the visitor provided no criteria, don't bother searching
            if (searchKeyword == string.Empty)
                lblSearchString.Text = CommonText.get("search criteria") + CommonText.get("search no criteria");
            else
            {
                // Remind the visitor what they provided as search criteria
                lblSearchString.Text = CommonText.get("search criteria") + searchKeyword;

                // Display the search results                
                gvSearchResults.DataSourceID = objDSResultContent.UniqueID;
                gvSearchResults.DataBind();
            }
        }
        protected void gvSearchResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
            Literal ltText = e.Row.FindControl("scTitle") as Literal;
            if (ltText != null)
            {
                if (((Item)e.Row.DataItem).Fields["Title"] != null && !string.IsNullOrEmpty(((Item)e.Row.DataItem).Fields["Title"].Value))
                {
                    ltText.Text = ((Item)e.Row.DataItem).Fields["Title"].Value;
                }
                else
                    ltText.Text = ((Item)e.Row.DataItem).Name;
            }
        }
        protected void btnSearchContent_Click(object sender, EventArgs e)
        {
            string newSearchURL = string.Empty;
            try
            {
                newSearchURL += LinkManager.GetItemUrl(Sitecore.Context.Item);
                newSearchURL += Sitecore.StringUtil.EnsurePrefix('?', "searchStr");
                newSearchURL += Sitecore.StringUtil.EnsurePrefix('=', txtKeywords.Text.Trim());
                Response.Redirect(newSearchURL, false);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString(), this);
            }
        }
        protected void objDSResultContent_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue != null)
            {
                string resultString = string.Empty;
                System.Collections.Generic.List<Item> obj = (System.Collections.Generic.List<Item>)e.ReturnValue;
                resultString += Sitecore.Globalization.Translate.Text("Results");
                resultString += " (";
                resultString += obj.Count().ToString();
                resultString += ")";
                litRecordsCount.Text = resultString;
                if (obj.Count() == 0)
                {
                    litRecordsCount.Text = string.Empty;
                }
            }
        }
    }
}