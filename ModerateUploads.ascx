<%@ Control Language="vb" Inherits="DotNetNuke.Modules.Repository.ModerateUploads" Codebehind="ModerateUploads.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<asp:Table ID="RepTable" Width="600" runat="server" BorderWidth="0" CssClass="normal" CellPadding="0" CellSpacing="0">
    <asp:TableRow Width="100%" VerticalAlign="Top">
        <asp:TableCell Width="100%" HorizontalAlign="Center" VerticalAlign="Top" CssClass="Head">
            <asp:Label runat="server" ID="lbTitle" CssClass="SubHead" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow Width="100%" VerticalAlign="Top">
        <asp:TableCell Width="100%" HorizontalAlign="Left" VerticalAlign="Top">
            <asp:Label runat="server" ID="lblNoRecords" CssClass="SubHead" Visible="False">
                <font color="red">
                    <br />
                    <br />
                    <asp:Label runat="server" ID="lbNoFiles" CssClass="SubHead" />
                    <br />
                </font>
            </asp:Label>
            <asp:DataGrid ID="lstObjects" runat="server" AllowSorting="True" Width="100%" AllowPaging="True"
                BorderWidth="0" OnPageIndexChanged="lstObjects_PageIndexChanged" ItemStyle-CssClass="normal"
                AlternatingItemStyle-CssClass="normal" AutoGenerateColumns="False" PageSize="5"
                PagerStyle-Visible="False">
                <HeaderStyle CssClass="normal" />
                <FooterStyle CssClass="normal" />
                <Columns>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:Label ID="TheFileName" runat="server" Visible="False">
								<%# DataBinder.Eval(Container.DataItem,"FileName")%>
                            </asp:Label>
                            <asp:Table BorderStyle="Solid" BorderColor="DimGray" BorderWidth="1" Width="100%"
                                CellPadding="4" CellSpacing="0" runat="server" CssClass="normal" ID="ItemTable">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left" VerticalAlign="Top" CssClass="Head" BackColor="DimGray">
                                        <asp:HyperLink ID="Hyperlink1" ImageUrl="~/images/edit.gif" NavigateUrl='<%# EditURL("ItemID",DataBinder.Eval(Container.DataItem,"ItemID")) %>'
                                            Visible="<%# IsEditable %>" runat="server" />
                                        <font color="white">
                                            <%# DataBinder.Eval(Container.DataItem,"Name") %>
                                        </font>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                            <asp:Table BorderStyle="Solid" BorderColor="DimGray" BorderWidth="1" Width="100%"
                                CellPadding="4" CellSpacing="0" runat="server" CssClass="normal" ID="ItemDetailsTable">
                                <asp:TableRow Width="100%" ID="ItemRow" runat="server">
                                    <asp:TableCell HorizontalAlign="center" VerticalAlign="Top" CssClass="normal" BackColor="white" runat="server">
                                        <asp:HyperLink ID="hlImage" runat="server" /><br />
                                        <asp:Label ID="lbClickToView" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left" VerticalAlign="Top" CssClass="normal" ID="tcDetails" runat="server">
                                        <asp:Label ID="lblItemDetails" runat="server"  />
                                        <br />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                            <asp:Table BorderStyle="Solid" BorderColor="DimGray" BorderWidth="1" Width="100%"
                                CellPadding="4" CellSpacing="0" runat="server" CssClass="normal" ID="ItemButtonTable">
                                <asp:TableRow Width="100%" ID="ItemButtonRow" runat="server">
                                    <asp:TableCell HorizontalAlign="Right" VerticalAlign="Middle" CssClass="SubHead"
                                        BackColor="LightGrey" ID="ItemButtonCell" runat="server">
                                        <asp:LinkButton ID="btnViewFile" runat="server" CssClass="SubHead" CommandName="ViewFile"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ItemID")%>'>OPEN FILE</asp:LinkButton>
                                        &nbsp;&nbsp;
                                        <asp:LinkButton ID="btnApprove" runat="server" CssClass="SubHead" CommandName="Approve"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ItemID")%>'>APPROVE</asp:LinkButton>
                                        &nbsp;&nbsp;
                                        <asp:LinkButton ID="btnReject" runat="server" CssClass="SubHead" CommandName="Reject"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ItemID")%>'>REJECT</asp:LinkButton><br />
                                        <!-- Rejection Table -->
                                        <asp:Table Width="100%" CellPadding="4" CellSpacing="0" runat="server" CssClass="normal" ID="tblReject" Visible="False">
                                            <asp:TableRow Width="100%">
                                                <asp:TableCell CssClass="SubHead">
                                                    <asp:Label runat="server" ID="lbRejectionNotice" CssClass="SubHead" />
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow Width="100%" ID="tblRejectionRow" runat="server">
                                                <asp:TableCell HorizontalAlign="Left" VerticalAlign="top" CssClass="normal" ID="tblRejectionCell" runat="server">
                                                    <span class="normal">
                                                        <b>
                                                            <asp:Label runat="server" ID="lbRejectionReason" />
                                                        </b><br />
                                                        <asp:TextBox ID="txtReason" Width="490" runat="server" TextMode="MultiLine" CssClass="NormalTextBox"
                                                            Rows="4" />
                                                        <br />
                                                        <asp:Button ID="btnSendRejection" runat="server" CssClass="normal" Text="SEND REJECTION NOTICE"
                                                            CommandName="SendRejection" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ItemID") %>'>
                                                        </asp:Button>
                                                    </span>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <br />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow BackColor="#E9E9E9" Height="25px" Width="100%">
        <asp:TableCell CssClass="SubHead" HorizontalAlign="Right" Width="100%">
            <asp:LinkButton ID="lnkPrev" runat="server" CommandArgument="prev">[<< BACK]</asp:LinkButton>
            &nbsp;&nbsp;
            <asp:Label ID="PagerText" runat="server" CssClass="normal"  />&nbsp;&nbsp;
            <asp:LinkButton ID="lnkNext" runat="server" CommandArgument="next">[NEXT >>]</asp:LinkButton>&nbsp;
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow Height="25px" Width="100%">
        <asp:TableCell CssClass="SubHead" HorizontalAlign="Left" Width="100%">
            <asp:LinkButton ID="btnReturn" runat="server" CommandArgument="Return">Return</asp:LinkButton>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
