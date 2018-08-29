<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_RegistrationSources.ascx.cs" Inherits="SALTShaker.usercontrol.uc_RegistrationSources" %>
                                    <%@ Register src="uc_WarningMessage.ascx" tagname="uc_WarningMessage" tagprefix="uc1" %>
                                    <style type="text/css">
                                        .style2
                                        {
                                            width: 78px;
                                        }
                                        .style5
                                        {
                                            width: 297px;
                                        }
                                    </style>
                                    <asp:GridView ID="RegistrationSourceGridView" AllowPaging="True" 
    AllowSorting="True" AutoGenerateColumns="False"
                                        runat="server" 
    BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px"
                                        CellPadding="2" ForeColor="Black" 
    GridLines="None" Width="100%" 
    OnPageIndexChanging="RegistrationSourceGridView_PageIndexChanging"  
    OnSelectedIndexChanging ="RegistrationSourceGridView_SelectedIndexChanging" 
    AutoGenerateSelectButton="True" >
                                        <AlternatingRowStyle BackColor="PaleGoldenrod" />
                                        <FooterStyle BackColor="Tan" />
                                        <HeaderStyle BackColor="Tan" Font-Bold="True" />
                                        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
                                            HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#54c400" ForeColor="GhostWhite" 
                                            BorderColor="#CC026E" />
                                        <SortedAscendingCellStyle BackColor="#FAFAE7" />
                                        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                                        <SortedDescendingCellStyle BackColor="#E1DB9C" />
                                        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                                        <Columns>                                            
                                            <asp:BoundField DataField="CampaignName" HeaderText="Campaign Name" />
                                            <asp:BoundField DataField="ChannelName" HeaderText="Channel Name" />
                                            <asp:BoundField DataField="RegistrationSourceName" 
                                                HeaderText="Registration Source Name" />
                                            <asp:BoundField DataField="RegistrationSourceID" HeaderText="Registration Source ID" />
                                            <asp:BoundField DataField="RegistrationDetail" HeaderText="Execution Detail" />
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" />
                                            <asp:BoundField DataField="ModifiedBy" HeaderText="Modified By" />
                                            <asp:BoundField DataField="ModifiedDate" HeaderText="Modified Date" />
                                            <asp:BoundField DataField="RefChannelId" HeaderText="RefChannelId" 
                                                ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" >
                                                <HeaderStyle CssClass="hiddencol"></HeaderStyle>
                                                <ItemStyle CssClass="hiddencol"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="RefCampaignId" HeaderText="RefCampaignId"
                                                ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" >
                                                <HeaderStyle CssClass="hiddencol"></HeaderStyle>
                                                <ItemStyle CssClass="hiddencol"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="RefRegistrationSourceTypeID" HeaderText="RefRegistrationSourceTypeId" 
                                                ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" >
                                                <HeaderStyle CssClass="hiddencol"></HeaderStyle>
                                                <ItemStyle CssClass="hiddencol"></ItemStyle>
                                            </asp:BoundField>
                                        </Columns>
                                    </asp:GridView>
                                
                                <p style="height: 25px">
                           <asp:Button runat="server" ID="cmdAddUpdateRefCampaign" Text="Add/Update Campaign" CssClass="small button radius"
                                OnClick="cmdAddUpdateRefCampaign_Click" CausesValidation="false" 
                                        UseSubmitBehavior="false" />
                            &nbsp
                            <asp:Button runat="server" ID="cmdAddRegSource" Text="Add Registration Source" CssClass="small button radius"
                                OnClick="cmdAddRegSource_Click" CausesValidation="false" 
                                        UseSubmitBehavior="false" />
                                    &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                    <br />
</p>
                                
                                <asp:Panel ID="NewRefCampaignPanel" 
    runat="server" Visible="false">
                                    <table id="tblCommunication">
                                        <tr>
                                            <th style="text-align: center" colspan="2">
                                                Create a new Campaign or update an existing one.</th>
                                        </tr>
                                        <tr>
                                            <th style="text-align: center" colspan="2">
                                                To add a Campaign to the SALT Database please input the campaign name and click 
                                                &#39;Add&#39;</th>
                                        </tr>
                                        <tr>
                                            <th style="text-align: center" colspan="2">
                                                To update a Campaign in the SALT Database select a campaign, update the campaign name and click &#39;Update&#39;</th>
                                        </tr>
                                        <tr>
                                            <td class="style5">
                                                Campaign Name:</td>
                                            <td class="style2">
                                                <asp:TextBox ID="txtCampaignName" runat="server" Height="24px" 
                                                    MaxLength="100" 
                                                    ToolTip="Campaign Names can be alpha/numeric. Note: The database allows up to 100 characters." 
                                                    Width="330px"></asp:TextBox>
                                            </td>                                            
                                        </tr>
                                        <tr>
                                            <td class="style5">
                                                Campaign:</td>
                                            <td class="style2">
                                                <asp:DropDownList ID="DDLRefCampaign_Upsert" runat="server" 
                                                    onselectedindexchanged="DDLRefCampaign_Upsert_SelectedIndexChanged" 
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                                <asp:Button ID="CmdSubmit_Upsert" runat="server" CausesValidation="false" 
                                                    CssClass="small button radius" OnClick="btnSubmit_Campaign_Click" Text="Add" 
                                                    UseSubmitBehavior="false" Visible="true"/>
                                                &nbsp
                                                <asp:Button ID="CmdUpdate_Upsert" runat="server" CausesValidation="false" 
                                                    CssClass="small button radius" OnClick="btnUpdate_Campaign_Click" Text="Update" 
                                                    UseSubmitBehavior="false" Visible="true" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="NewRegSourcePanel" 
    runat="server" Visible="false">
                                    <table id="Table1">
                                        <tr>
                                            <th style="text-align: center" colspan="4">
                                                Create A New Registration Source</th>
                                        </tr>
                                        <tr>
                                            <th style="text-align: center" colspan="4">
                                                To add a new Registration Source to the SALT Database please input the information below and click &#39;Submit&#39;</th>
                                        </tr>
                                        <tr>
                                            <td class="style5">
                                                Registration Source Name:</td>
                                            <td class="style2">
                                                <asp:TextBox ID="txtRegSourceName" runat="server" Height="24px" 
                                                    MaxLength="100" 
                                                    ToolTip="Registration Source Name can be alpha/numeric.  Examples include: RapidReg, MeetMe, BeanPot15.  Note: The database allows up to 100 characters." 
                                                    Width="330px"></asp:TextBox>
                                            </td>                                            
                                        </tr>
                                        <tr>
                                            <td class="style5">
                                                <asp:Label ID="Label1CommSource" runat="server" 
                                                    Text="Execution Detail:"></asp:Label>
                                            </td>
                                            <td class="style2">
                                                <asp:TextBox ID="txtRegSourceDetail" runat="server" Height="24px" 
                                                    MaxLength="1000" 
                                                    ToolTip="Execution Detail can be alpha/numeric. Examples include: Registration via Rapid Registration form, Registration via MeetMe campaign, Registration via Bean Pot event.  Note: The database allows up to 1000 characters." 
                                                    Width="330px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style5">
                                                Campaign:</td>
                                            <td class="style2">
                                                <asp:DropDownList ID="DDLRefCampaign" runat="server" 
                                                    onselectedindexchanged="DDLRefCampaign_SelectedIndexChanged" 
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style5">
                                                Channel:</td>
                                            <td class="style2">
                                                <asp:DropDownList ID="DDLRefChannel" runat="server" 
                                                    onselectedindexchanged="DDLRefChannel_SelectedIndexChanged" 
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style5">
                                                Registration Source Type:</td>
                                            <td class="style2">
                                                <asp:DropDownList ID="DDLRegSourceType" runat="server" 
                                                    onselectedindexchanged="DDLRegSourceType_SelectedIndexChanged" 
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                                <asp:Button ID="CmdSubmit" runat="server" CausesValidation="false" 
                                                    CssClass="small button radius" OnClick="btnSubmit_Click" Text="Submit" 
                                                    UseSubmitBehavior="false" />
                                                <asp:Button ID="CmdUpdate" runat="server" CausesValidation="false" 
                                                    CssClass="small button radius" OnClick="btnUpdate_Click" Text="Update" 
                                                    UseSubmitBehavior="false" Visible="False" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                
                                
                        <uc1:uc_WarningMessage ID="warningMessageControl" 
    runat="server" />
                        
                            
                                
                                
