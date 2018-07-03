<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DashboardSettings.ascx.vb" Inherits="DotNetNuke.Modules.Repository.DashboardSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<TABLE cellSpacing="0" cellPadding="0" style="border: solid 0px #fff;">
	<TR vAlign="top">
		<TD class="normal"><SPAN class="SubHead">
			<dnn:label id="plRepository" runat="server" suffix=":"></dnn:label></SPAN><BR>
			<asp:DropDownList ID="ddlRepositoryID" Runat="server" Width="300px" CssClass="normal"></asp:DropDownList>
			<br /><br />
		</TD>
	</TR>
	<TR vAlign="top">
		<TD class="normal"><SPAN class="SubHead">
			<dnn:label id="plStyle" runat="server" suffix=":"></dnn:label></SPAN><BR>
			<asp:RadioButtonList ID="rbStyle" Runat="server" CssClass="normal" RepeatDirection="Vertical">
				<asp:ListItem Value="index">Category Listing (1 column)</asp:ListItem>
				<asp:ListItem Value="categories">Category Listing (multi-column)</asp:ListItem>
				<asp:ListItem Value="latest">Latest Uploads</asp:ListItem>
				<asp:ListItem Value="top10Downloads">Top Downloads</asp:ListItem>
				<asp:ListItem Value="top10Rated">Top Rated</asp:ListItem>
			</asp:RadioButtonList><BR>
			<BR>
		</TD>
	</TR>
	<TR vAlign="top">
		<TD class="normal"><SPAN class="SubHead">
			<dnn:label id="plCount" runat="server" suffix=":"></dnn:label></SPAN><BR>
			<asp:TextBox id="txtRowCount" runat="server" Width="100" cssclass="NormalTextBox" maxlength="150"></asp:TextBox><BR>
			<BR>
		</TD>
	</TR>
</TABLE>
<asp:Label id="lblMessage" Runat="server" CssClass="normalred"></asp:Label>
