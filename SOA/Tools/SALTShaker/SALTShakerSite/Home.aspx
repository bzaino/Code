<%@ Page Title="SaltShaker" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Home" Codebehind="Home.aspx.cs" %>
<%@ Register src="usercontrol/uc_Home.ascx" tagname="uc_Home" tagprefix="uc2" %>

<asp:Content ID="HomeContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <uc2:uc_Home ID="uc_Home1" runat="server" />
</asp:Content>

