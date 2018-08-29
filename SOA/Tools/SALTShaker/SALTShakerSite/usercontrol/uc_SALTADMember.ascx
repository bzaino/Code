<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_SALTADMember.ascx.cs" Inherits="SALTShaker.usercontrol.uc_SALTADMember" %>

<%@ Register src="uc_WarningMessage.ascx" tagname="uc_WarningMessage" tagprefix="uc1" %>
<asp:Panel ID="SaltUSerSearchPanel" runat="server" DefaultButton="cmdGetUserinfo">
<div>
    <section class="email_search_container">
	        <div id="DivBoxSearch" class="admin_search">
	        	<div class="row">
	        		<!-- label and search -->
	        		<div class="columns large-10">
	        			<div class="input_elements row collapse blocked">
			                <div class="columns large-2">
                                <span class="user_label">
			                        <asp:Label ID="lblPrompt" runat="server">
			                            <strong>User email:</strong>
			                        </asp:Label>
                                </span>
			                </div>
			                <div class="columns large-10">
			                    <asp:TextBox ID="TextUserEmailAddress" CssClass="search" runat="server" ></asp:TextBox> 
			                </div>    
			            </div>
			            <!-- /label and search -->
	        		</div>
	        		
	        		<!-- Submit -->
	        		<div class="columns large-2">
	        			<asp:Button ID="cmdGetUserinfo" runat="server" Text="search" CssClass="button blank radius"  onclick="cmdGetUserinfo_Click" />
	        		</div>
	        		<!-- Submit -->
	        	</div>
	        </div>
	    </section>

    <uc1:uc_WarningMessage ID="uc_UserPrompMessage" runat="server" />
</div>

<asp:PlaceHolder ID="PHAlertbox" runat="server" Visible="false">
       <section class="table_buffer">
	        <table id="tblPHAlertbox" class="admin_search">
	        	<tr>
                    <td> 
			            <asp:Label ID="LblAlertMsg" runat="server">
			                <strong>Are you sure you want to deactivate this user?</strong>
			            </asp:Label>
                    </td>
                    <td> 
                        <asp:Button ID="cmdYes" runat="server" Text="Yes" CssClass="button blank radius"  onclick="cmdYes_Click" />
                    </td>
                    <td>
                         <asp:Button ID="cmdNo" runat="server" Text="No" CssClass="button blank radius"  onclick="cmdNo_Click" />
                    </td>
                </tr>
	        </table>
	    </section>
</asp:PlaceHolder>

<asp:PlaceHolder ID="PHListUserInfo" runat="server" visible ="false">

    <table>
            <thead>
                <tr>
                    <th class="th_cap">Mail | Username</th>
                    <th >Name</th>
                    <th >Creation Date</th>
                    <th >Environment Name</th>
                    <th >User PrincipalName</th>
                    <th >CN</th>
                </tr>   
            </thead>
            <tbody>
                 <tr>
                    <td > <a href="/AD/InvokeWebservice?Length=4" uri="Production" ID="hrefLink" runat="server" ><asp:Label ID="SaltUserEmail" runat="server"></asp:Label></a></td>
                    <td ><asp:Label ID="SaltUserName" runat="server"></asp:Label></td>
                    <td ><asp:Label ID="DateOfCreation" runat="server"></asp:Label></td>
                    <td  ><asp:Label ID="UserEnvironmentName" runat="server" ForeColor="Red"></asp:Label></td>
                    <td ><asp:Label ID="UserPrincipalName" runat="server"></asp:Label></td>
                    <td ><asp:Label ID="CN" runat="server"></asp:Label></td>
                 </tr> 
                 <tr>
                    <%--<td >
                        <span class="user_delete">
                            <asp:Button ID="cmdToggleActivate" runat="server" CssClass="button blank" Text="Deactivate user" onclick="cmdToggleActivate_Click"/>
                        </span>
                    </td>--%>
                    <td >
                        <span class="user_delete">
                            <asp:Button ID="cmdDeleteADUser" runat="server" CssClass="button blank" Text="Deactivate User" onclick="cmdDeleteADUser_Click" />
                        </span>
                    </td> 
                 </tr>
            </tbody>
    </table>

</asp:PlaceHolder>

</asp:Panel>
