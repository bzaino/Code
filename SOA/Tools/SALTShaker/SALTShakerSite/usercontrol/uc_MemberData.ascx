<%@ Control Language="C#" AutoEventWireup="true" Inherits="SALTShaker.usercontrol.uc_MemberData"
    CodeBehind="uc_MemberData.ascx.cs" %>
<%@ Register Src="uc_WarningMessage.ascx" TagName="uc_WarningMessage" TagPrefix="uc1" %>
<section class="table_wrap">
    <header class="wrap_title">
        <h6 class="title">
            Salt User Search</h6>
    </header>
    <!-- buffer-->
    <div class="table_buffer">
        <!-- Loading Spinner -->
        <div class="loading" align="center">
            <img src="../Images/SaltShaker-loader.gif" alt="" />
        </div>
        <asp:UpdatePanel ID="UpdatePanelMemberGrid" runat="server">
            <ContentTemplate runat="server">
                <div id="msgholder" class="style1">
                    <uc1:uc_WarningMessage ID="warningMessageControl" runat="server" />
                </div>
                <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="1">
                    <asp:View ID="View1" runat="server">
                        <asp:GridView ID="MemberDataGridView" ShowHeaderWhenEmpty="True" runat="server" AllowPaging="True"
                            AllowSorting="True" AutoGenerateColumns="False" CellPadding="0" ForeColor="#333333"
                            GridLines="None" OnSelectedIndexChanged="MemberDataGridView_SelectedIndexChanged"
                            OnPageIndexChanging="MemberDataGridView_PageIndexChanging" OnRowDataBound="MemberDataGridView_RowDataBound"
                            CssClass="searchresult_list" EnableModelValidation="False" DataKeyNames="MemberID,EmailAddress" OnSorting="MemberDataGridView_Sorting">
                            <Columns>
                                <asp:ButtonField CommandName="Select" Visible="true" />
                                <asp:BoundField DataField="MemberID" HeaderText="MemberID" SortExpression="MemberID"
                                    NullDisplayText="Prospect" />
                                <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                                <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
                                <asp:BoundField DataField="EmailAddress" HeaderText="EmailAddress" SortExpression="EmailAddress" />
                                <asp:BoundField DataField="OrganizationName" HeaderText="OrganizationName" 
                                    SortExpression="OrganizationName" />
                            </Columns>
                            <SelectedRowStyle BackColor="#FF0080" ForeColor="White" />
                        </asp:GridView>
                        <br />
                        <div>
                            <asp:Panel ID="pnl_SearchResultCount" runat="server" Visible="False">
                                <asp:Label ID="lbl_SearchResultCount" runat="server"></asp:Label>
                            </asp:Panel>
                        </div>
                    </asp:View>
                    <asp:View ID="View2" runat="server">
                        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchBtn">
                            <table class="table_search">
                                <tr>
                                    <td class="text-right search-field">
                                        *Member ID:
                                    </td>
                                    <td class="search-field">
                                        <asp:TextBox ID="TextBoxMemberID" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-right search-field">
                                        First Name:
                                    </td>
                                    <td class="search-field">
                                        <asp:TextBox ID="TextBoxFirstName" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-right search-field">
                                        Last Name:
                                    </td>
                                    <td class="search-field">
                                        <asp:TextBox ID="TextBoxLastName" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-right search-field">
                                        *Email Address:
                                    </td>
                                    <td class="search-field">
                                        <asp:TextBox ID="TextBoxEmail" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-right search-field">
                                        <asp:Button ID="SearchBtn" runat="server" CssClass="small button radius" OnClick="SearchBtn_Click"
                                            OnClientClick="DBTool.Spinner.show();" Text="Search" />
                                    </td>
                                    <td>
                                        <asp:Button ID="Reset1" runat="server" class="small button radius" Text="Clear Fields"
                                            OnClick="ClearSearch_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        * Indicates exact match search
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</section>
