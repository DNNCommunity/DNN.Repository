<%@ Control language="vb" Inherits="DotNetNuke.Modules.Repository.RepositoryDashboard" CodeBehind="RepositoryDashboard.ascx.vb" AutoEventWireup="False" Explicit="True" %>
<div id="repository_dashboard_content" class="repository_dashboard_content">
<asp:Table id="DashTable" Width="100%" Runat="server" CssClass="normal" CellSpacing="0" CellPadding="0"
	BorderWidth="0">
	<asp:TableRow Width="100%" VerticalAlign="Top">
		<asp:TableCell Width="100%" HorizontalAlign="Left" VerticalAlign="Top">
			<asp:DataGrid id="lstObjects" runat="server" Width="100%" BorderWidth="0" ItemStyle-CssClass="normal"
				AlternatingItemStyle-CssClass="normal" AutoGenerateColumns="False" Visible="True" EnableViewState="True"
				 style="border-collapse: separate;" GridLines="None">
				<HeaderStyle CssClass="normal"></HeaderStyle>
				<FooterStyle CssClass="normal"></FooterStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemTemplate>
							<asp:PlaceHolder ID="Placeholder" Runat="server" Visible="True"></asp:PlaceHolder>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			<asp:DataList id="datList" runat="server" Width="100%" RepeatLayout="Table" RepeatDirection="Horizontal"
				RepeatColumns="3" ItemStyle-Wrap="false" ItemStyle-VerticalAlign="Top" Visible="True" EnableViewState="True">
				<ItemStyle Wrap="False" VerticalAlign="Top"></ItemStyle>
				<ItemTemplate>
					<asp:PlaceHolder ID="Placeholder2" Runat="server" Visible="True"></asp:PlaceHolder>
				</ItemTemplate>
			</asp:DataList>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
</div>