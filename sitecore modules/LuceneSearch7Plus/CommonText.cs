using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Search.UI.LuceneSearch
{
    public class CommonText
    {
        private static readonly Database masterDB;

        static CommonText()
        {
            masterDB = Factory.GetDatabase("master");
        }

        public static string get(string name)
        {
            Item commonText = masterDB.GetItem("/sitecore/content/Lucene Search Settings/common text/" + name);
            return commonText == null ? null : commonText["text"];
        }
    }//class
}