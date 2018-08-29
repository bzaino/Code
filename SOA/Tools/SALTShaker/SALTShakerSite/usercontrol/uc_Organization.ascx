<%@ Control Language="C#" AutoEventWireup="True" Inherits="usercontrol_uc_Organization" CodeBehind="uc_Organization.ascx.cs" %>
<%@ Register Src="uc_WarningMessage.ascx" TagName="uc_WarningMessage" TagPrefix="uc1" %>
<section class="table_wrap">
    <header class="wrap_title">
        <h6 class="title">
            Organization Search</h6>
    </header>
    <!-- buffer-->
    <div class="table_buffer">
        <!-- Loading Spinner -->
        <div class="loading" align="center">
            <img src="../Images/SaltShaker-loader.gif" alt="" />
        </div>
        <asp:UpdatePanel ID="UpdatePanelMemberGrid" runat="server">
            <ContentTemplate>
                <div id="msgholder">
                    <uc1:uc_WarningMessage ID="warningMessageControl" runat="server" />
                </div>
                <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="1">
                    <asp:View ID="View1" runat="server">
                        <asp:GridView ID="OrganizationGridView" runat="server" AllowPaging="True" AllowSorting="True"
                            AutoGenerateColumns="False" CellPadding="0"
                            ForeColor="#333333" GridLines="None" OnSelectedIndexChanged="OrganizationGridView_SelectedIndexChanged"
                            OnPageIndexChanging="OrganizationGridView_PageIndexChanging" OnRowDataBound="OrganizationGridView_RowDataBound"
                            CssClass="searchresult_list" EnableModelValidation="False" OnSorting="OrganizationGridView_Sorting"
                            DataKeyNames="RefOrganizationID">
                            <Columns>
                                <asp:ButtonField CommandName="Select" Visible="true" />
                                <asp:BoundField DataField="OrganizationName" HeaderText="Organization Name " 
                                    SortExpression="OrganizationName" />
                                <asp:BoundField DataField="OrganizationAliases" HeaderText="Organization Alias" 
                                    SortExpression="OrganizationAliases" />
                                <asp:BoundField DataField="OPECode" HeaderText="OE Code " SortExpression="OPECode" />
                                <asp:BoundField DataField="BranchCode" HeaderText="Branch Code" SortExpression="BranchCode" />
                                <asp:BoundField DataField="OrganizationExternalId" HeaderText="Organization ID" 
                                    SortExpression="OrganizationExternalID" />
                                <asp:TemplateField HeaderText="Contracted" SortExpression="IsContracted">
                                    <ItemTemplate>
                                        <%# (Boolean.Parse(Eval("IsContracted").ToString())) ? "Yes" : "No"%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <SelectedRowStyle BackColor="#FF0080" ForeColor="White" />
                        </asp:GridView>
                    </asp:View>
                    <asp:View ID="View2" runat="server">
                        <div>
                            <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton">
                                <table class="table_search">
                                    <tr>
                                        <td class="text-right search-field">
                                            Organization Name:
                                        </td>
                                        <td class="search-field">
                                            <asp:TextBox ID="TextBoxOrganizationName" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-right search-field">
                                            *OE Code:
                                        </td>
                                        <td class="search-field">
                                            <asp:TextBox ID="TextBoxOpeCode" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-right search-field">
                                            *Branch Code:
                                        </td>
                                        <td class="search-field">
                                            <asp:TextBox ID="TextBoxBranchCode" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="RowRefOrganizationID">
                                        <td class="text-right search-field">
                                            *Organization ID:
                                        </td>
                                        <td class="search-field">
                                            <asp:TextBox ID="TextBoxRefOrganizationID" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-right search-field">
                                            Contracted :
                                        </td>
                                        <td class="search-field">
                                            <asp:DropDownList ID="DropDownListContracted" runat="server" CssClass="select-field">
                                                <asp:ListItem Selected="True">Please Select</asp:ListItem>
                                                <asp:ListItem Value="True">Yes</asp:ListItem>
                                                <asp:ListItem Value="False">No</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="text-right search-field">
                                            <asp:Button ID="SearchButton" runat="server" CssClass="small button radius" OnClick="SearchButton_Click"
                                                Text="Search" OnClientClick="DBTool.Spinner.show();" />
                                        </td>
                                        <td class="search-field">
                                            <asp:Button ID="ResetSearch1" runat="server" class="small button radius" OnClick="ClearSearch_Click"
                                                Text="Clear Fields" />
                                        </td>
                                    </tr>
                                    <tr><td colspan="4">* Indicates exact match search</td></tr>
                                </table>
                            </asp:Panel>
                        </div>
                    </asp:View>
                </asp:MultiView>
                <asp:Label ID="RecordNoLabel" runat="server" Text="RecordNoLabel" Visible="False"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</section>
