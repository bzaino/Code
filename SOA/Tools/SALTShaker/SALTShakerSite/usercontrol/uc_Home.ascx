<%@ Control Language="C#" AutoEventWireup="true" Inherits="usercontrol_uc_Home" CodeBehind="uc_Home.ascx.cs" %>
<%@ Register src="uc_SALTADMember.ascx" tagname="uc_SALTADMember" tagprefix="uc2" %>
<%@ Register src="uc_WarningMessage.ascx" tagname="uc_WarningMessage" tagprefix="uc1" %>
<section class="table_wrap">
    <header class="wrap_title">
        <h6 class="title">Welcome</h6>
    </header>
    <div class="table_buffer">

    			<figure>
    				<img src="../Images/logo_shaker.png" alt="Salt Shaker" />
    			</figure>

    </div>
</section>
<uc1:uc_WarningMessage ID="uc_WarningMessage1" runat="server" />
