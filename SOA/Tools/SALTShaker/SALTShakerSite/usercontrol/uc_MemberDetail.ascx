<%@ Control Language="C#" AutoEventWireup="true" Inherits="usercontrol_uc_MemberDetail" CodeBehind="uc_MemberDetail.ascx.cs" %>
<%@ Register Src="uc_WarningMessage.ascx" TagName="uc_WarningMessage" TagPrefix="uc1" %>
<script type="text/javascript">
    
    var make_tab_active = function () {
        //Get item siblings
        var siblings = ($(this).siblings());

        //Remove active class on all buttons
        siblings.each(function (index) {
            $(this).children().removeClass('selectedTab');
        });

        //Add the clicked button class
        $(this).children().addClass('selectedTab');
    }

    //Attach events to menu
    $(document).ready(
        function () {
            //asp dynamically generated list items for the tabs
            $("li[role='menuitem']").click(make_tab_active);
        }
    )

    $(document).ready(
        function () {
            //trigger the Organizations tab click to be rendered by default
            $("li[role='menuitem'] > a:contains('Organizations')").trigger('click');
        }
    )
    
</script>
<br />
<table id="tblParent">
    <!-- Member Detail block-->
    <tr>
        <td class="leftalign">
            <!-- Member Detail table-->
            <table id="tblMemberInfo">
                <tr>
                    <th class="text-center" colspan="4">
                        Member Detail
                    </th>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToMemberID" runat="server" Text="Member ID:"></asp:Label>
                    </td>
                    <td >
                        <asp:Label ID="LabelMemberID" runat="server" ></asp:Label>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToFirst" runat="server" Text="First Name:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelFN" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToLastName" runat="server" Text="Last Name:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelLN" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToEmailAdd" runat="server" Text="Email Address:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelEA" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToGradeLevel" runat="server" Text="Grade Level:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelGL" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToEnrollment" runat="server" Text="Enrollment Status:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelES" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToActivationDate" runat="server" Text="Activation Date:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelActivationDate" runat="server"></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToMemberAcitve" runat="server" Text="Is Member Active:"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="CheckBoxActive" CssClass="checkLabel" runat="server" Text="Activated"
                            Enabled="True" AutoPostBack="True" Checked="True" OnCheckedChanged="CheckBoxActive_CheckedChanged" />
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToDeactivationDate" runat="server" Text="Deactivation Date:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelDeactivationDate" runat="server"></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="LabelNextToModifiedByDisplay" runat="server" Text="Modified By:"></asp:Label>
                    </td>
                    <td class="text-left">
                        <asp:Label ID="LabelModifiedBy" runat="server"></asp:Label>
                    </td>
                </tr>
                </table>
        </td>
    </tr>
    <tr>
        <td>
            <asp:PlaceHolder ID="PHAlertbox" runat="server" Visible="false">
                <section class="table_buffer">
                    <table id="tblPHAlertbox" class="admin_search" onclick="return tblPHAlertbox_onclick()">
                        <tr>
                            <td>
                                <asp:Label ID="LblAlertMsg" runat="server"> <strong>Are you sure you want to 
			        deactivate this user?</strong> </asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="cmdYes" runat="server" CssClass="button blank radius" OnClick="cmdYes_Click"
                                    Text="Yes" />
                            </td>
                            <td>
                                <asp:Button ID="cmdNo" runat="server" CssClass="button blank radius" OnClick="cmdNo_Click"
                                    Text="No" />
                            </td>
                        </tr>
                    </table>
                </section>
            </asp:PlaceHolder>
        </td>
    </tr>
    <!-- Grid block-->
    <tr>
        <td colspan="6">
            <asp:Menu ID="detailTabMenu" Width="100%" runat="server" Orientation="Horizontal"
                OnMenuItemClick="tabMenu1_MenuItem_Click" Height="22px" StaticSubMenuIndent="10px"
                BackColor="#E3EAEB" DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="0.8em"
                ForeColor="#666666">
                <DynamicHoverStyle BackColor="#666666" ForeColor="White" />
                <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                <DynamicMenuStyle BackColor="#E3EAEB" />
                <DynamicSelectedStyle BackColor="#1C5E55" />
                <Items>
                    <asp:MenuItem Text="Organizations" Value="0"></asp:MenuItem>
                    <asp:MenuItem Text="User Roles" Value="1"></asp:MenuItem>
                </Items>
                <StaticHoverStyle BackColor="#666666" ForeColor="White" />
                <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            </asp:Menu>
        </td>
    </tr>
    <tr>
        <td colspan="4">
            <div id="dynamicContainer">
                <asp:UpdatePanel ID="ContainerUpdatePanel" runat="server">
                    <ContentTemplate>
                        <!-- the number of views is the index value used by MenuItem above starting at zero -->
                        <asp:MultiView ID="MultiViewRelatedContent" runat="server" ActiveViewIndex="0">
                            <asp:View ID="ViewOrganizations" runat="server">
                                <asp:Panel ID="OrganizationsGridViewPanel" runat="server">
                                    <asp:GridView ID="OrganizationsGridView" AllowPaging="True" AllowSorting="True"
                                        AutoGenerateColumns="False" runat="server" BackColor="LightGoldenrodYellow" BorderColor="Tan"
                                        BorderWidth="1px" CellPadding="2" ForeColor="Black" GridLines="None" Width="100%"
                                        OnPageIndexChanging="OrganizationsGridView_PageIndexChanging" OnSorting="OrganizationsGridView_Sorting">
                                        <AlternatingRowStyle BackColor="PaleGoldenrod" />
                                        <FooterStyle BackColor="Tan" />
                                        <HeaderStyle BackColor="Tan" Font-Bold="True" />
                                        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                                        <SortedAscendingCellStyle BackColor="#FAFAE7" />
                                        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                                        <SortedDescendingCellStyle BackColor="#E1DB9C" />
                                        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                                        <Columns>
                                            <asp:BoundField DataField="OrganizationName" HeaderText="Organization Name" 
                                                SortExpression="OrganizationName" />
                                            <asp:BoundField DataField="OrganizationExternalID" HeaderText="Organization ID" 
                                                SortExpression="OrganizationExternalID" />
                                            <asp:CheckBoxField DataField="IsContracted" HeaderText="Sponsored" SortExpression="IsContracted" />
                                            <asp:BoundField DataField="EffectiveStartDate" HeaderText="Date Affiliated" 
                                                SortExpression="EffectiveStartDate" />
                                            <asp:BoundField DataField="EffectiveEndDate" HeaderText="End Date" 
                                                SortExpression="EffectiveEndDate" />
                                            <asp:BoundField DataField="ExpectedGraduationYear" 
                                                HeaderText="Expected GraduationYear" SortExpression="ExpectedGraduationYear" />
                                            <asp:BoundField DataField="ReportingID" 
                                                HeaderText="Reporting ID" SortExpression="ReportingID" />
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="ViewRoles" runat="server">
                                <asp:Panel ID="RolesGridViewPanel" runat="server">
                                    <asp:GridView ID="RolesGridView" AllowPaging="True" AllowSorting="True"
                                        AutoGenerateColumns="False" runat="server" BackColor="LightGoldenrodYellow" BorderColor="Tan"
                                        BorderWidth="1px" CellPadding="2" ForeColor="Black" GridLines="None" Width="100%"
                                        OnPageIndexChanging="RolesGridView_PageIndexChanging" OnSorting="RolesGridView_Sorting">
                                        <AlternatingRowStyle BackColor="PaleGoldenrod" />
                                        <FooterStyle BackColor="Tan" />
                                        <HeaderStyle BackColor="Tan" Font-Bold="True" />
                                        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                                        <SortedAscendingCellStyle BackColor="#FAFAE7" />
                                        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                                        <SortedDescendingCellStyle BackColor="#E1DB9C" />
                                        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                                        <Columns>
                                            <asp:BoundField DataField="RoleName" HeaderText="Role Name" />
                                            <asp:BoundField DataField="RoleDescription" HeaderText="Description" ReadOnly="True" >
                                            <ControlStyle Height="25px" Width="220px" />
                                            <ItemStyle Width="220px" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Active"  >
                                               <ItemTemplate>
                                                  <asp:CheckBox ID="IsMemberRoleActive" OnCheckedChanged="chkview_CheckedChanged" runat="server"   Checked='<%# Eval("IsMemberRoleActive") %>' onClick="$('.isdirtycheck').val( 'true' )" />
                                               </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Assigned By" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Assigned Date" />
                                            <asp:BoundField DataField="ModifiedBy" HeaderText="Modified By" />
                                            <asp:BoundField DataField="ModifiedDate" HeaderText="Modified Date" />
                                            <asp:TemplateField ShowHeader="false" >
                                             <ItemTemplate>
                                                  <asp:TextBox ID="RefMemberRoleID" runat="server"  Text ='<%# Eval("RefMemberRoleID") %>' Visible=false  />
                                               </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </asp:View>
                        </asp:MultiView>
                        <uc1:uc_WarningMessage ID="warningMessageControl" runat="server" />
                        <div id="divButtonContainer">
                            <asp:Button runat="server" ID="CmdSaveChanges" Text="Save Changes" CssClass="small button radius"
                                OnClientClick="DBTool.Controls.clearAllDirtyFlags();" OnClick="CmdSaveChanges_Click"
                                CausesValidation="false" UseSubmitBehavior="false" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="detailTabMenu" EventName="MenuItemClick" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <input id="isDirtyHidden" class="isdirtycheck" type="hidden" runat="server" value="false" />
        </td>
    </tr>
</table>
<br />
<br />
<script type="text/javascript">
    var needToConfirm = true;
    window.onbeforeunload = confirmExit;
    function confirmExit() {
        if (needToConfirm && $('.isdirtycheck').val() == 'true') {
            return "You have attempted to leave this page.  If you have made any changes to the fields without clicking the Save button, your changes will be lost.";
        }
    }
</script>
