<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" Inherits="Registration" Codebehind="Registration.aspx.cs" %>

<%@ Register src="usercontrol/uc_Registration.ascx" tagname="uc_Registration" tagprefix="uc1" %>

    <%--<link href="CSS/foundation.css" rel="stylesheet" type="text/css" />--%>

<asp:Content ID="HomeContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <uc1:uc_Registration ID="uc_Registration1" runat="server" />
</asp:Content>
   
    
