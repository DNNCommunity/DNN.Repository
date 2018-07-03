<%@ Control Language="vb" AutoEventWireup="false" Codebehind="About.ascx.vb" Inherits="DotNetNuke.Modules.Repository.AboutRepository" TargetSchema="http://schemas.microsoft.com/intellisense/ie3-2nav3-0" %>
<br />
<table cellspacing="0" cellpadding="8" width="500" align="center" border="1">
    <tr>
        <td>
            <table cellspacing="0" cellpadding="0" border="0" class="normal" width="100%">
                <tr valign="top" bgcolor="lightgrey">
                    <td class="Head" align="center">
                        <asp:Label ID="plAboutHeader" runat="server" CssClass="subhead" />
                     </td>
                </tr>
                <tr valign="top">
                    <td class="normal">
                        <p>
                            <br />
                            <asp:Label ID="plAbout" runat="server" CssClass="normal" />
                            <br />
                            <asp:Label ID="plSupport" runat="server" CssClass="normal" />
                        </p>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <br />
                        <asp:Button ID="btnCancel" runat="server" CssClass="normal" Text="Return" CausesValidation="False" />
                        <br />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
