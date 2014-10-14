
using Sitecore;
using Sitecore.Search;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.ComponentModel;
using Search.UI.LuceneSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.SearchTypes;
using System.Linq.Expressions;
using System;
using Sitecore.ContentSearch.Linq.Utilities;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using Sitecore.Configuration;
using Sitecore.Xml.XPath;
using Sitecore.ContentSearch.Utilities;
using System.Linq;
using Sitecore.Diagnostics;

namespace Search.UI.LuceneSearch
{
    [DataObject]
    public class SearchManager
    {
        public const string OtherCategory = "Other";

        private readonly Item SiteRoot;                     // The item associated with the site root
        private string SearchIndexName { get; set; }           // The search index for the current database (assumed to be the master DB)

        public SearchManager()
        { }
        public SearchManager(string indexName)
        {
            SearchIndexName = indexName;
            Database database = Factory.GetDatabase("master");
            var item = Sitecore.Context.Site.StartPath;
            SiteRoot = database.GetItem(item);
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<Item> GetSearchResultsUsingQuery(string searchString)
        {
            //Getting index from the web.config
            List<Item> returnValues = new List<Item>();
            SearchIndexName = StringUtil.GetString(SearchIndexName, CommonText.get("Search Index")); ;
            ISearchIndex searchIndex = null;
            try
            {
                searchIndex = ContentSearchManager.GetIndex(SearchIndexName);
            }
            catch (Exception ex)
            {

                Log.Error(ex.Message, ex);
                return null;
            }


            using (var context = searchIndex.CreateSearchContext())
            {
                IQueryable<SearchUIResultItem> q = context.GetQueryable<SearchUIResultItem>();
                List<SearchResults<SearchUIResultItem>> LstResults = new List<SearchResults<SearchUIResultItem>>();

                // //Query approach
                q = BuildSearchQuery(searchString, q);
                LstResults.Add(q.GetResults());

                List<string> uniqueIds = new List<string>();
                foreach (var results in LstResults)
                {
                    var sitecoreItem = results.Hits.Select(x => x.Document);
                    foreach (var item in sitecoreItem)
                    {
                        Item item1 = item.GetItem();
                        if (item1 != null && !uniqueIds.Contains(item1.ID.ToString()))
                        {
                            returnValues.Add(item1);
                            uniqueIds.Add(item1.ID.ToString());
                        }
                    }
                }
                return returnValues;
            }
        }

        # region Predicate Method
        public List<Item> GetSearchResultsUsingPredicate(string searchString)
        {
            //Getting index from the web.config
            List<Item> returnValues = new List<Item>();
            SearchIndexName = StringUtil.GetString(SearchIndexName, CommonText.get("Search Index")); ;
            ISearchIndex searchIndex = null;
            try
            {
                searchIndex = ContentSearchManager.GetIndex(SearchIndexName);
            }
            catch (Exception ex)
            {

                Log.Error(ex.Message, ex);
                return null;
            }
            using (var context = searchIndex.CreateSearchContext())
            {                
                var predicate = BuildPredicate(searchString);
                IQueryable<SearchResultItem> q = context.GetQueryable<SearchResultItem>()                    
                    .Where(predicate);
                List<SearchResults<SearchResultItem>> LstResults = new List<SearchResults<SearchResultItem>>();
                LstResults.Add(q.GetResults());
                List<string> uniqueIds = new List<string>();
                foreach (var results in LstResults)
                {
                    var sitecoreItem = results.Hits.Select(x => x.Document);
                    foreach (var item in sitecoreItem)
                    {
                        Item item1 = item.GetItem();
                        if (item1 != null && !uniqueIds.Contains(item1.ID.ToString()))
                        {
                            returnValues.Add(item1);
                            uniqueIds.Add(item1.ID.ToString());
                        }
                    }
                }
                return returnValues;
            }
        }
        # endregion Predicate Method
        private Expression<Func<SearchResultItem, bool>> BuildPredicate(string searchString)
        {
            var predicate = PredicateBuilder.True<SearchResultItem>();
            foreach (var str in searchString.Split(' '))
            {
                predicate = predicate.Or(p => p.Content.Contains(str));
                // Add extra relevance if term matches item name
                predicate = predicate.Or(p => p.Name.Contains(str)).Boost(2f);
            }

            //predicate = predicate.And(p => p.GetItem); // Only get pages that have renderings
            predicate = predicate.And(p => p.Language == Sitecore.Context.Language.Name);

            return predicate;
        }
        private IQueryable<SearchUIResultItem> BuildSearchQuery(string searchKeyword, IQueryable<SearchUIResultItem> query)
        {
            return query = query.Where(x => x.Content.Contains(searchKeyword)
                || x.Name.Contains(searchKeyword) && (x.Language == Sitecore.Context.Language.Name));
        }
        public static string RemoveSpecialCharacters(string keyword)
        {
            List<string> CharsToRemove = new List<string>() { "\'", "%", "<", ">", "!", "~", "@", "$", "^", "&", "*", "(", ")", "#", "_", "-", "+", "=", "'" };
            foreach (var character in CharsToRemove)
            {
                keyword = keyword.Replace(character, string.Empty);
            }
            return keyword;
        }
        public static string GetFoundWordParagraph(string keywords, string contentText, bool highLightKeyword)
        {

            XmlDocument xmlDocument = new XmlDocument();
            string[] keywordsArrery = null;
            StringBuilder sbReturn = new StringBuilder(string.Empty);
            string foundFirstParagraph = string.Empty;
            try
            {
                contentText = contentText.Replace(System.Environment.NewLine, string.Empty);
                contentText = contentText.Replace("\t", string.Empty);

                if (!string.IsNullOrEmpty(contentText))
                {
                    xmlDocument.LoadXml("<contents><content><![CDATA[" + contentText.Replace("&nbsp;", " ") + "]]></content></contents>");
                    keywords = RemoveSpecialCharacters(keywords);
                    keywordsArrery = keywords.Split(' ');
                    foreach (string kWord in keywordsArrery)
                    {
                        // search for the first Paragraph on the whole HTML body content
                        XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("./descendant::*[contains(translate(text(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'" + kWord.ToLower() + "')]");

                        if (xmlNodeList != null & xmlNodeList.Count > 0)
                        {
                            foundFirstParagraph = Regex.Replace(xmlNodeList[0].InnerText, kWord, "<span class='" + CommonText.get("Highlighter CSS Class") + "'>" + kWord + "</span>", RegexOptions.IgnoreCase);
                        }
                    }
                }
                return foundFirstParagraph;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xmlDocument = null;
                sbReturn = null;
            }
        }
    }
    public class SearchUIResultItem : SearchResultItem
    {
        [IndexField("content")]
        public string content { get; set; }

        [IndexField("title")]
        public string title { get; set; }

        [IndexField("metakeywords")]
        public string metakeywords { get; set; }

        [IndexField("Text")]
        public string text { get; set; }

        public bool HasUrl { get; set; }
    }
}