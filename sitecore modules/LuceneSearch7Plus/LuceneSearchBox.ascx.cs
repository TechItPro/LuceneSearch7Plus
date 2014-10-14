
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Links;
using System;
using System.Drawing;
using System.Web.UI;
namespace Search.UI.LuceneSearch
{
    public partial class LuceneSearchBox : System.Web.UI.UserControl
    {
        private Database masterDB = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            masterDB = Factory.GetDatabase("master");
            if (!IsPostBack)
                txtCriteria.Text = CommonText.get("search");

           
        }

        protected void btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (txtCriteria.Text != CommonText.get("search"))
                performSearch();
        }

        protected void txtCriteria_TextChanged(object sender, EventArgs e)
        {
            if (txtCriteria.Text != CommonText.get("search"))
                performSearch();
        }

        private void performSearch()
        {
            Database database = Factory.GetDatabase("master");
            var home = database.GetItem(Sitecore.Context.Site.StartPath);

            if (home != null)
            {
                var results = home.Axes.SelectSingleItem("./Standard_Items/Search_Results");

                if (results != null)
                {

                    string results_url = LinkManager.GetItemUrl(results) + "?searchStr=" + txtCriteria.Text;
                    Response.Redirect(results_url);
                }
                else
                {
                    txtCriteria.ForeColor = Color.Red;
                    txtCriteria.Text = "Unable to find results item";
                }
            }
            else
            {
                txtCriteria.ForeColor = Color.Red;
                txtCriteria.Text = "Unable to find home!";
            }


        }
    }
}