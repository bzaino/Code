<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="OrganizationDetail - SALTShaker" AutoEventWireup="true" CodeBehind="OrganizationDetail.aspx.cs" Inherits="SALTShaker.OrganizationDetail" %>

<%@ Register src="usercontrol/uc_OrganizationDetail.ascx" tagname="uc_OrganizationDetail" tagprefix="uc1" %>
<asp:Content ID="DefaultContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <link href="CSS/custom.css" rel="stylesheet" type="text/css" />   
    <uc1:uc_OrganizationDetail ID="uc_OrganizationDetail" runat="server" />
   
</asp:Content>
