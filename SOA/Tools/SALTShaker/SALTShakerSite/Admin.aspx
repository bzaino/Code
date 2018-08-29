<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Admin" Title="SaltShaker" Codebehind="Admin.aspx.cs" %>

<%@ Register src="usercontrol/uc_AdminForm.ascx" tagname="uc_AdminForm" tagprefix="uc1" %>

<asp:Content ID="AdminMainContent" ContentPlaceHolderID="MainContent" Runat="Server">

    <uc1:uc_AdminForm ID="uc_AdminForm1" runat="server" />

</asp:Content>



