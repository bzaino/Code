<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_OrganizationDetail.ascx.cs" Inherits="SALTShaker.usercontrol.uc_OrganizationDetail" %>
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

        if ($(this).text() === 'To-Do Content') {
            toggleAddToDoArea(true);
        }
        else {
            toggleAddToDoArea(false);
        }                
    }

    function toggleAddToDoArea(showSection) {
        if (showSection) {
            $('#MainContent_uc_OrganizationDetail_btnAddOrgToDo').removeClass('hidden');
            $('#toDoInfo').removeClass('hidden');
            $('#dueDateInfo').removeClass('hidden');
        }
        else {
            $('#MainContent_uc_OrganizationDetail_btnAddOrgToDo').addClass('hidden');
            $('#toDoInfo').addClass('hidden');
            $('#toDoErrorRow').addClass('hidden');
            $('#dueDateInfo').addClass('hidden');
        }
    }

    //Attach events to menu
    $(document).ready(
        function () {
            //asp dynamically generated list items for the tabs
            $("li[role='menuitem']").click(make_tab_active);

            if ($('#MainContent_uc_OrganizationDetail_TodoContentGridViewPanel').is(':visible')) {
                toggleAddToDoArea(true);
            }
        }
    )

    function isPostBack() {
        return document.referrer.indexOf(document.location.href) > -1;
    }

    $(document).ready(
        function () {
            if (isPostBack()) {
                //trigger the Products tab click to be rendered by default
                $("li[role='menuitem'] > a.selectedTab").trigger('click');
                //highlight back the selected tab
                $("li[role='menuitem'] > a.selected").addClass('selectedTab');
                $('#MainContent_uc_OrganizationDetail_txtOrgToDo').val('');
                $('#MainContent_uc_OrganizationDetail_txtToDoDate').val('');
                if ($('#MainContent_uc_OrganizationDetail_isDirtyHiddenAddToDoText').val() == 'true') {
                    $('#MainContent_uc_OrganizationDetail_isDirtyHiddenAddToDoText').val('false');
                }
            }
            else {
                //trigger the Products tab click to be rendered by default
                $("li[role='menuitem'] > a:contains('Products')").trigger('click');
            }

            $('[id$=txtToDoDate]').datepicker({
                minDate: new Date()
            });
        }
    )

    function validateToDo() {
        if ($('#MainContent_uc_OrganizationDetail_txtOrgToDo').val() === '') {
            $('#toDoErrorRow').removeClass('hidden');
            return false;
        }
        else {
            $('#toDoErrorRow').addClass('hidden');
            return true;
        }
    }

</script>
<br />
<table>
    <!-- Organization Detail block-->
    <tr>
        <td class="leftalign">
            <!-- Organization Detail table-->
            <table>
                <tr>
                    <th class="text-center" colspan="4">
                        <asp:Label ID="LabelOrgNameHeader" runat="server" Text="">Organization Name (Placeholder)</asp:Label>
                    </th>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderOrgname" runat="server" Text="Organization Name:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelOrgname" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderOrgAlia" runat="server" Text="Organization Alias:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelOrgAlias" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderOECode" runat="server" Text="OE Code:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelOECode" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderBranchCode" runat="server" Text="Branch Code:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelBranchCode" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderLogoName" runat="server" Text="Logo Name:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelOrganizationLogo" runat="server"></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderContracted" runat="server" Text="Contracted:"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="CheckBoxIsContracted" CssClass="checkLabel" runat="server" 
                            Enabled="false" oncheckedchanged="SetDirtyFlagTrue" />
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderOrgID" runat="server" Text="Organization ID:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelOrgID" runat="server" Text="" Visible="false"></asp:Label>
                        <asp:Label ID="LabelExtOrgID" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderAllowedLookUp" runat="server" Text="Is Allowed in Look up:"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="CheckBoxAllowedLookupFlag" CssClass="checkLabel" runat="server"
                            Enabled="true" 
                            oncheckedchanged="SetDirtyFlagTrue" onClick="$('.isdirtycheck').val( 'true' )"  />
                    </td>
                </tr>
                <tr>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderModifiedBy" runat="server" Text="Modified By:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelModifiedBy" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="text-right">
                        <asp:Label ID="PlaceHolderModifiedDate" runat="server" Text="Modified Date:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LabelModifiedDate" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <!-- Grid block-->
    <tr>
        <td>
            <asp:Menu ID="OrganizationDetailTabMenu" Width="100%" runat="server" Orientation="Horizontal"
                Height="22px" StaticSubMenuIndent="10px" BackColor="#E3EAEB" DynamicHorizontalOffset="2"
                Font-Names="Verdana" Font-Size="0.8em" ForeColor="#666666" OnMenuItemClick="tabMenu1_MenuItem_Click">
                <DynamicHoverStyle BackColor="#666666" ForeColor="White" />
                <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                <DynamicMenuStyle BackColor="#E3EAEB" />
                <DynamicSelectedStyle BackColor="#1C5E55" />
                <StaticHoverStyle BackColor="#666666" ForeColor="White" />
                <Items>
                    <asp:MenuItem Text="Products" Value="0"></asp:MenuItem>
                    <asp:MenuItem Text="Courses" Value="1"></asp:MenuItem>
                    <asp:MenuItem Text="To-Do Content" Value="2"></asp:MenuItem>
                </Items>
                <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            </asp:Menu>
        </td>
    </tr>
    <tr>
        <td>
            <div id="dynamicContainer">
                <asp:UpdatePanel ID="OrganizationDetailPanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:MultiView ID="detailMultiView" runat="server" ActiveViewIndex="0">
                            <asp:View ID="ViewProductDetail" runat="server">
                                <asp:Panel ID="ProductGridViewPanel" runat="server">
                                    <asp:GridView ID="ProductGridView" AllowPaging="True" 
                                        AllowSorting="True" AutoGenerateColumns="False"
                                        runat="server" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px"
                                        HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" CellPadding="2"
                                        ForeColor="Black" GridLines="None" Width="100%" OnPageIndexChanging="ProductGridView_PageIndexChanging">
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
                                            <asp:BoundField DataField="ProductName" HeaderText="Product Name" />
                                            <asp:BoundField DataField="ProductDescription" HeaderText="Description" ReadOnly="True" >
                                            <ControlStyle Height="25px" Width="220px" />
                                            <ItemStyle Width="220px" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Active"  >
                                               <ItemTemplate>
                                                  <asp:CheckBox ID="IsRefOrganizationProductActive" OnCheckedChanged="chkview_CheckedChanged" runat="server"   Checked='<%# Eval("IsRefOrganizationProductActive") %>' onClick="$('.isdirtycheck').val( 'true' )" />
                                               </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" />
                                            <asp:BoundField DataField="ModifiedBy" HeaderText="Modified By" />
                                            <asp:BoundField DataField="ModifiedDate" HeaderText="Modified Date" />
                                            <asp:TemplateField ShowHeader="false" >
                                             <ItemTemplate>
                                                  <asp:TextBox ID="RefProductID" runat="server"  Text ='<%# Eval("RefProductID") %>' Visible=false  />
                                               </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="CoursesView" runat="server">
                                <asp:Panel ID="CoursesViewPanel" runat="server">
                                    <asp:GridView ID="CoursesGridView" AllowPaging="True" 
                                        AllowSorting="True" AutoGenerateColumns="False"
                                        runat="server" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px"
                                        HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" CellPadding="2"
                                        ForeColor="Black" GridLines="None" Width="100%" OnPageIndexChanging="CoursesGridView_PageIndexChanging">
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
                                            <asp:BoundField DataField="ProductName" HeaderText="Product Name" />
                                            <asp:BoundField DataField="ProductDescription" HeaderText="Description" ReadOnly="True" >
                                            <ControlStyle Height="25px" Width="220px" />
                                            <ItemStyle Width="220px" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Active"  >
                                               <ItemTemplate>
                                                  <asp:CheckBox ID="IsRefOrganizationProductActive" OnCheckedChanged="chkview_CheckedChanged" runat="server"   Checked='<%# Eval("IsRefOrganizationProductActive") %>' onClick="$('.isdirtycheck').val( 'true' )" />
                                               </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" />
                                            <asp:BoundField DataField="ModifiedBy" HeaderText="Modified By" />
                                            <asp:BoundField DataField="ModifiedDate" HeaderText="Modified Date" />
                                            <asp:TemplateField ShowHeader="false" >
                                             <ItemTemplate>
                                                  <asp:TextBox ID="RefProductID" runat="server"  Text ='<%# Eval("RefProductID") %>' Visible=false  />
                                               </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="ViewTodoContent" runat="server">
                                <asp:Panel ID="TodoContentGridViewPanel" runat="server">
                                    <asp:GridView ID="TodoContentGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
                                        BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" CellPadding="2" ForeColor="Black" GridLines="None" Width="100%" 
                                        OnPageIndexChanging="TodoContentGridView_PageIndexChanging" OnSelectedIndexChanged="TodoContentGridView_SelectedIndexChanged" 
                                        DataKeyNames="ContentId, TypeId, StatusId" OnSorting=TodoContentGridView_Sorting>
                                        <AlternatingRowStyle BackColor="PaleGoldenrod" />
                                        <FooterStyle BackColor="Tan" />
                                        <HeaderStyle BackColor="Tan" Font-Bold="True" />
                                        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="Gray" ForeColor="GhostWhite" />
                                        <SortedAscendingCellStyle BackColor="#FAFAE7" />
                                        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                                        <SortedDescendingCellStyle BackColor="#E1DB9C" />
                                        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                                        <Columns>
                                            <asp:BoundField DataField="Headline" HeaderText="Headline" SortExpression="Headline" />
                                            <asp:BoundField DataField="PageTitle" HeaderText="Page Title" SortExpression="PageTitle" />
                                            <asp:BoundField DataField="ContentType" HeaderText="Content Type" SortExpression="ContentType" />
                                            <asp:TemplateField HeaderText="To-Do Type">
                                                <ItemTemplate>
                                                    <asp:Label ID="RefToDoType" runat="server" Text='<%#ConvertTypeIdToText(Eval("TypeId")) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="To-Do Status">
                                                <ItemTemplate>
                                                    <asp:Label ID="RefToDoStatus" runat="server" Text='<%#ConvertStatusIdToText(Eval("StatusId")) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Created By"  />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:M/dd/yyyy}" />
                                            <asp:BoundField DataField="DueDate" HeaderText="Due Date" DataFormatString="{0:M/dd/yyyy}" />
                                            <asp:ButtonField Text="Select" CommandName="Select" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:LinkButton Text="" ID = "lnkFake" runat="server" />                                  
                                    <ajaxtoolkit:ModalPopupExtender ID="modalPopup" runat="server" PopupControlID="pnlToDoPopup" TargetControlID="lnkFake"
                                    CancelControlID="btnCloseToDo" BackgroundCssClass="modalBackground" DropShadow="True">
                                    </ajaxtoolkit:ModalPopupExtender>
                                    <asp:Panel ID="pnlToDoPopup" runat="server" CssClass="modalPopupPanel" Style="display: none">
                                        <div>
                                            Details
                                        </div>
                                        <div>
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td style = "width:60px">
                                                        <b>Headline: </b>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtUpdateHeadline" runat="server" onChange="DBTool.Controls.toggleUpdateToDoTextDirtyFlagValue()" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <b>Due Date: </b>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtUpdateDueDate" runat="server" onChange="DBTool.Controls.toggleUpdateToDoDateDirtyFlagValue()" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <b>To-Do Status: </b>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUpdateToDoStatus" runat="server" onChange="DBTool.Controls.toggleUpdateToDoStatusDirtyFlagValue()">
                                                            <asp:ListItem Text="Added" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="Cancelled" Value="3"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <input type="hidden" id="toDoId" runat="server" />
                                                <input type="hidden" id="toDoType" runat="server" />
                                                <input id="isDirtyHiddenUpdateToDoText" class="isdirtycheckUpdateToDoText" type="hidden" runat="server" value="false" />
                                                <input id="isDirtyHiddenUpdateToDoDate" class="isdirtycheckUpdateToDoDate" type="hidden" runat="server" value="false" />
                                                <input id="isDirtyHiddenUpdateToDoStatus" class="isdirtycheckUpdateToDoStatus" type="hidden" runat="server" value="false" />
                                            </table>
                                        </div>
                                        <div align="right">
                                            <asp:Button ID="btnUpdateToDo" runat="server" Text="Update" CssClass="small button radius" OnClick="btnUpdateToDo_Click" CausesValidation="true"/>
                                            <asp:Button ID="btnCloseToDo" runat="server" Text="Close" CssClass="small button radius" />
                                        </div>
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:View>
                           </asp:MultiView>
                        <uc1:uc_WarningMessage ID="warningMessageControl" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="OrganizationDetailTabMenu" EventName="MenuItemClick" />
                        
                    </Triggers>
                </asp:UpdatePanel>
                <br />
            </div>
        </td>
    </tr>
    <tr>
        <td class="text-left search-field"><asp:Button ID="Savechanges" runat="server" CssClass="small button radius" 
                OnClientClick="$('.isdirtycheck').val( 'false' ) " Text="Save changes" 
                onclick="Savechanges_Click" CausesValidation="False" UseSubmitBehavior="false"/>
            
            <asp:Button ID="btnAddOrgToDo" runat="server" Text="Add To Do" CssClass="small button radius hidden" 
                OnClientClick="$('.isdirtycheck').val( 'false' ); if (!validateToDo()) return false;"
                onclick="btnAddOrgToDo_Click" CausesValidation="False" UseSubmitBehavior="false" />
            
        </td>
    </tr>
    <tr class="hidden" id="toDoErrorRow">
        <td>
            <div class="alert-box alert" id="toDoErrorText">Please enter text for your To Do item</div>
        </td>
    </tr>
    <tr>
        <td> <input id="isDirtyHidden" class="isdirtycheck" type="hidden" runat="server" value="false" />         
             <input id="isDirtyHiddenAddToDoText" class="isdirtycheckAddToDoText" type="hidden" runat="server" value="false" />
             <input id="isDirtyHiddenAddToDoDate" class="isdirtycheckAddToDoDate" type="hidden" runat="server" value="false" />

            <div id="toDoInfo" class="hidden">
                <asp:Label ID="Label1" runat="server" Text="To Do Item"></asp:Label>
                <asp:TextBox ID="txtOrgToDo" runat="server" Width="470px" CssClass="hidden" onChange="DBTool.Controls.toggleAddToDoTextDirtyFlagValue()"></asp:TextBox>
            </div>
        </td>
    </tr>
    <tr>
        <td> 
            
            <div id="dueDateInfo" class="hidden">
                <asp:Label ID="Label4" runat="server" Text="Due Date"></asp:Label>
                <asp:TextBox ID="txtToDoDate" runat="server" Width="100px" CssClass="hidden" onChange="DBTool.Controls.toggleAddToDoDateDirtyFlagValue()"></asp:TextBox>
            </div>
        </td>
    </tr>
    
</table>
<div class="loading" align="center">
            <img src="../Images/SaltShaker-loader.gif" alt="" />
</div>
<script type="text/javascript">
    var needToConfirm = true;
    window.onbeforeunload = confirmExit;
    function confirmExit() {
        if (needToConfirm && $('.isdirtycheck').val() == 'true') {
            return "You have attempted to leave this page.  If you have made any changes to the fields without clicking the Save button, your changes will be lost.";
        }
    }
</script>
