<%@ Control Language="C#" AutoEventWireup="true" Inherits="usercontrol_uc_LoginForm" Codebehind="uc_LoginForm.ascx.cs" %>
<%--<form id="formLogin" runat="server">--%>

    <span>Your session is expired, please 
<asp:HyperLink ID="HyperLinkRedirect"
        runat="server" NavigateUrl="~/Welcome.aspx">Click here</asp:HyperLink> to revisit the site.</span>

<%--</form>--%>
