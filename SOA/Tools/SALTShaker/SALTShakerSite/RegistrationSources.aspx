<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="RegistrationSources.aspx.cs" 
Title="SaltShaker" Inherits="SALTShaker.RegistrationSources" %>
<%@ Register Src="~/usercontrol/uc_RegistrationSources.ascx" TagPrefix="uc" TagName="uc_RegistrationSources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <link href="CSS/custom.css" rel="stylesheet" type="text/css" />
    <uc:uc_RegistrationSources runat="server" ID="uc_RegistrationSources" />
</asp:Content>