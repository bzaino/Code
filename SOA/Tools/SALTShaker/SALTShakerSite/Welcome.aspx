<%@ Page Title="Salt Shaker" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Welcome.aspx.cs" Inherits="SALTShaker.Welcome" %>
<%@ Register Src="~/usercontrol/uc_WarningMessage.ascx" TagName="uc_WarningMessage" TagPrefix="uc1" %>

<asp:Content ID="WelcomeContent" ContentPlaceHolderID="MainContent" Runat="Server">

        <section class="welcome_block row">
    		<div class="columns large-8 large-centered">
    			<div class="row">
    				<section class="columns large-8">
    					<figure>
    						<img src="Images/logo_shaker.png" alt="Salt Shaker" />
    					</figure>
    				</section>
    				<section class="columns large-4 welcome_cta">
                        <asp:HyperLink ID="EnterButton" CssClass="button large radius" NavigateUrl="MemberData.aspx" runat="server">Enter</asp:HyperLink>
                        <asp:Label ID="LabelMessage" runat="server" Text="Your account is not authorized to use Salt Shaker." ForeColor="#d6d6d6"></asp:Label>
    				</section>
    			</div>
    		</div>
		</section>

</asp:Content> 

 

