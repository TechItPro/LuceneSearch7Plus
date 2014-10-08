<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LuceneSearchBox.ascx.cs" Inherits="Search.UI.LuceneSearch.LuceneSearchBox" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<script type="text/javascript">
    function doClear(inputField) {
        if (inputField.value == inputField.defaultValue) {
            inputField.value = "";
            inputField.style.color = "black";
        }
    }
</script>
<asp:Panel ID="SearchPanel" runat="server" DefaultButton="btnSearch" class="search-panel">
    <div class="search-box">
        <table cellpadding="0px">
            <tr>
                <td valign="top">
                    <asp:TextBox onFocus="doClear(this)" runat="server" ID="txtCriteria" Width="130px" OnTextChanged="txtCriteria_TextChanged" ForeColor="Silver"></asp:TextBox></td>
                <td>
                    <asp:ImageButton runat="server" ID="btnSearch" ImageUrl="/images/search.gif" OnClick="btnSearch_Click"></asp:ImageButton></td>
            </tr>
        </table>
    </div>
</asp:Panel>
