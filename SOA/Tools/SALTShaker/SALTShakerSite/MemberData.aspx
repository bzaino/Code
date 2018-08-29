<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="MemberData"
    Title="SaltShaker" CodeBehind="MemberData.aspx.cs" %>

<%@ Register Src="~/usercontrol/uc_MemberData.ascx" TagPrefix="uc" TagName="uc_MemberData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <link href="CSS/custom.css" rel="stylesheet" type="text/css" />
    <uc:uc_MemberData runat="server" ID="uc_MemberData1" />
</asp:Content>
