<%@ Control Language="C#" AutoEventWireup="true" Inherits="usercontrol_uc_AdminForm" Codebehind="uc_AdminForm.ascx.cs" %>
<%@ Register Src="uc_WarningMessage.ascx" TagName="uc_WarningMessage" TagPrefix="uc1" %>
<%@ Register src="uc_SALTADMember.ascx" tagname="uc_SALTADMember" tagprefix="uc2" %>
<section class="table_wrap">
    <header class="wrap_title">
        <h6 class="title">Welcome Admin</h6>
    </header>
    <!-- buffer-->
    <div class="table_buffer">

        <asp:UpdatePanel ID="UpdatePanelManageacount" runat="server">
            <ContentTemplate>
                <header class="headline">
                    <h3 class="user">
                        <span class="headline_cap">User:</span><asp:Label ID="LabelFirstName" runat="server" Text="nameLabel"></asp:Label>
                    </h3>
                    <h4 class="role">
                        <span class="headline_cap">Role:</span><asp:Label ID="LabelRole" runat="server" Text=""></asp:Label>
                    </h4>
                </header>
                <asp:PlaceHolder ID="PHRoleSelection" runat="server" ></asp:PlaceHolder>
                <uc1:uc_WarningMessage ID="uc_UserPrompMessage" runat="server" />
                <div id="DivSaltADMember">
                    <uc2:uc_SALTADMember ID="uc_SALTADMember1" runat="server" />
                    <br />
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
 

</div>
</section>         
          
<!-- section to be removed  -->
<div class="table_buffer">
    <asp:GridView ID="GridAdminView" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="0" 
        DataKeyNames="User_ID" DataSourceID="SqlDataSource1" ForeColor="" 
        GridLines="None" Width="100%" Visible="false" >
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:CommandField ShowEditButton="True" />
            <asp:BoundField DataField="User_ID" HeaderText="User_ID" InsertVisible="False" 
                ReadOnly="True" SortExpression="User_ID" />
            <asp:BoundField DataField="UserName" HeaderText="UserName" 
                SortExpression="UserName" />
            <asp:BoundField DataField="Password" HeaderText="Password" 
                SortExpression="Password" />
            <asp:BoundField DataField="EmailAddress" HeaderText="EmailAddress" 
                SortExpression="EmailAddress" />
            <asp:BoundField DataField="FullName" HeaderText="FullName" 
                SortExpression="FullName" />
            <asp:BoundField DataField="Depart" HeaderText="Depart" 
                SortExpression="Depart" />
            <asp:CheckBoxField DataField="IsAdmin" HeaderText="IsAdmin" 
                SortExpression="IsAdmin" />
            <asp:CheckBoxField DataField="IsActive" HeaderText="IsActive" 
                SortExpression="IsActive" />
        </Columns>
    </asp:GridView>
</div>
                            
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:SALTADMINToolContext1 %>" 
    DeleteCommand="DELETE FROM [MarketingMembership] WHERE [User_ID] = @User_ID" 
    InsertCommand="INSERT INTO [MarketingMembership] ([UserName], [Password], [EmailAddress], [FullName], [Depart], [IsAdmin], [IsActive]) VALUES (@UserName, @Password, @EmailAddress, @FullName, @Depart, @IsAdmin, @IsActive)" 
    SelectCommand="SELECT * FROM [MarketingMembership]" 
        
        
    UpdateCommand="UPDATE [MarketingMembership] SET [UserName] = @UserName, [Password] = @Password, [EmailAddress] = @EmailAddress, [FullName] = @FullName, [Depart] = @Depart, [IsAdmin] = @IsAdmin, [IsActive] = @IsActive WHERE [User_ID] = @User_ID">
    <DeleteParameters>
        <asp:Parameter Name="User_ID" Type="Int32" />
    </DeleteParameters>
    <InsertParameters>
        <asp:Parameter Name="UserName" Type="String" />
        <asp:Parameter Name="Password" Type="String" />
        <asp:Parameter Name="EmailAddress" Type="String" />
        <asp:Parameter Name="FullName" Type="String" />
        <asp:Parameter Name="Depart" Type="String" />
        <asp:Parameter Name="IsAdmin" Type="Boolean" />
        <asp:Parameter Name="IsActive" Type="Boolean" />
    </InsertParameters>
    <UpdateParameters>
        <asp:Parameter Name="UserName" Type="String" />
        <asp:Parameter Name="Password" Type="String" />
        <asp:Parameter Name="EmailAddress" Type="String" />
        <asp:Parameter Name="FullName" Type="String" />
        <asp:Parameter Name="Depart" Type="String" />
        <asp:Parameter Name="IsAdmin" Type="Boolean" />
        <asp:Parameter Name="IsActive" Type="Boolean" />
        <asp:Parameter Name="User_ID" Type="Int32" />
    </UpdateParameters>
</asp:SqlDataSource>