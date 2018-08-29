<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" Inherits="Index" Codebehind="Index.aspx.cs" %>
<%@ Register src="usercontrol/uc_LoginForm.ascx" tagname="uc_LoginForm" tagprefix="uc1" %>
<asp:Content ID="IndexContent" ContentPlaceHolderID="MainContent" Runat="Server">
   <!-- <asp:LoginName ID="CurrentUserLoginName" FormatString="Welcome {0} !Index" runat="server" />-->
    <uc1:uc_LoginForm ID="uc_LoginFormCtrl" runat="server" />
</asp:Content>