<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LuceneSearchResults.ascx.cs" Inherits="Search.UI.LuceneSearch.LuceneSearchResults" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<p>
    <asp:Label ID="lblSearchString" runat="server" Text="Label"></asp:Label>
    <asp:Panel ID="pnResultsPanel" runat="server"></asp:Panel>
</p>
<asp:Panel ID="pnlcriteria" runat="server" CssClass="search-criteria contet_block clear"
    DefaultButton="btnSearchContent">
    <p>
        <label>
            <%= Sitecore.Globalization.Translate.Text("Keyword:")%>
        </label>
        <asp:TextBox ID="txtKeywords" runat="server"></asp:TextBox>
    </p>

    <div class="search-buttons">
        <asp:Button ID="btnSearchContent" runat="server" Text="Search" OnClick="btnSearchContent_Click"
            CssClass="search_small" />
        &nbsp;
      
    </div>
    <div class="record-count">
        <asp:Literal ID="litRecordsCount" runat="server"></asp:Literal>
    </div>
</asp:Panel>
<asp:GridView ID="gvSearchResults" runat="server" AllowPaging="True" AutoGenerateColumns="False"
    GridLines="None" ShowFooter="false" ShowHeader="true" OnRowDataBound="gvSearchResults_RowDataBound">


    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <div style="border-bottom: 2px solid grey; width: 100%;">
                    <p>
                        <a href='<%# Container.DataItem != null ? Sitecore.Links.LinkManager.GetItemUrl((Sitecore.Data.Items.Item) Container.DataItem) : "#" %>'>
                            <asp:Literal ID="scTitle" runat="server" />
                        </a>
                    </p>
                    <div>
                        <p>
                            <%# Eval("Fields[\"Text\"].Value") != null ? Search.UI.LuceneSearch.SearchManager.GetFoundWordParagraph(txtKeywords.Text.Trim(), Eval("Fields[\"Text\"].Value").ToString(), true):string.Empty %>
                        </p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        No content found.
    </EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="objDSResultContent" runat="server" OldValuesParameterFormatString="original_{0}"
    SelectMethod="GetSearchResultsUsingPredicate" TypeName="Search.UI.LuceneSearch.SearchManager" OnSelected="objDSResultContent_Selected">
    <SelectParameters>
        <asp:ControlParameter ControlID="txtKeywords" Name="searchString" PropertyName="Text"
            Type="String" ConvertEmptyStringToNull="false" />
    </SelectParameters>
</asp:ObjectDataSource>














