<%@ Page MasterPageFile="~/Site.master" Language="C#" AutoEventWireup="true" Inherits="Detail" Title="MemberDetail - SALTShaker" Codebehind="Detail.aspx.cs" %>
<%@ Register src="usercontrol/uc_MemberDetail.ascx" tagname="uc_MemberDetail" tagprefix="uc1" %>
<asp:Content ID="DefaultContent" ContentPlaceHolderID="MainContent" Runat="Server">
   
    <uc1:uc_MemberDetail ID="uc_MemberDetail" runat="server" />
   
</asp:Content>