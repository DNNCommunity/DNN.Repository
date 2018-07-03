<%@ Control Language="vb" Codebehind="EditComment.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Repository.EditComment" %>
<table cellspacing="0" cellpadding="0" width="750" summary="Edit Repository Comment">
    <tr valign="top">
        <td class="SubHead">
            <label for="<%=txtName.ClientID%>">User Name:</label>
        </td>
        <td>
            <asp:TextBox ID="txtName" runat="server" MaxLength="100" Columns="30" Width="390" CssClass="NormalTextBox" />
            <br />
        </td>
    </tr>
    <tr valign="top">
        <td class="SubHead">
            <label for="<%=txtComment.ClientID%>">Comment:</label>
        </td>
        <td>
            <asp:TextBox ID="txtComment" runat="server" Columns="44" Width="390" CssClass="NormalTextBox" Rows="6" TextMode="Multiline" />
            <br />
        </td>
    </tr>
</table>
<p>
    <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="CommandButton" Text="Update" BorderStyle="none" />&nbsp;
    <asp:LinkButton ID="cmdCancel" runat="server" CssClass="CommandButton" Text="Cancel" BorderStyle="none" CausesValidation="False" />&nbsp;
    <asp:LinkButton ID="cmdDelete" runat="server" CssClass="CommandButton" Text="Delete" BorderStyle="none" CausesValidation="False" />&nbsp;
</p>
<hr size="1" noshade="noshade" />
