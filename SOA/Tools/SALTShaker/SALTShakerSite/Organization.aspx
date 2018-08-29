<%@ Page Title="Salt Shaker" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Organization" Codebehind="Organization.aspx.cs" %>
<%@ Register src="usercontrol/uc_Organization.ascx" tagname="uc_Organization" tagprefix="uc1" %>
<asp:Content ID="OrganizationContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <uc1:uc_Organization ID="uc_Organization" runat="server" />
</asp:Content>

