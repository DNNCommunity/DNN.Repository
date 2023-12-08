<%@ Control Language="vb" Inherits="DotNetNuke.Modules.Repository.Repository" Codebehind="Repository.ascx.cs" AutoEventWireup="False" Explicit="True" %>

<div id="repository_content" class="repository_content">

<asp:Label ID="lblDescription" runat="server" CssClass="normal" />

<asp:Label ID="lblTest" Runat="server" CssClass="normal" />

<asp:Table ID="HeaderTable" Width="100%" runat="server" CssClass="normal" CellSpacing="0" CellPadding="0"
	border="0">
	<asp:TableRow>
		<asp:TableCell>
			<asp:PlaceHolder ID="hPlaceholder" runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

<asp:Table ID="DataTable" Width="100%" runat="server" CssClass="normal" CellSpacing="0" CellPadding="0" style="border: solid 0px white;">
	<asp:TableRow>
		<asp:TableCell>
		
			<asp:DataGrid ID="lstObjects" runat="server" AllowSorting="True" Width="100%" AllowPaging="True"
				OnItemCommand="lstObjects_ItemCommand" BorderWidth="0" BorderStyle="None" OnPageIndexChanged="lstObjects_PageIndexChanged"
				ItemStyle-CssClass="normal" AlternatingItemStyle-CssClass="normal" AutoGenerateColumns="False"
				PageSize="5" PagerStyle-Visible="False" Visible="True" EnableViewState="True" Style="border-collapse: separate;"
				ShowHeader="False" GridLines="None">
				<HeaderStyle CssClass="normal" />
				<FooterStyle CssClass="normal" />
				<Columns>
					<asp:TemplateColumn>
						<ItemTemplate>
							<asp:Label ID="TheFileName" runat="server" Visible="False">
								<%# DataBinder.Eval(Container.DataItem, "FileName")%>
							</asp:Label>
							<asp:PlaceHolder ID="PlaceHolder" runat="server" Visible="False" />
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			
			<asp:DataList id="DataList1" Width="100%" runat="server" RepeatColumns="1" RepeatDirection="Horizontal" EnableViewState="True">
			    <ItemTemplate>
				    <asp:PlaceHolder ID="Placeholder1" runat="server" Visible="False" />	
			    </ItemTemplate>
			</asp:DataList>
			
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

<asp:Table ID="FooterTable" Width="100%" runat="server" CssClass="normal" CellSpacing="0" CellPadding="0">
	<asp:TableRow>
		<asp:TableCell>
			<asp:PlaceHolder ID="fPlaceHolder" runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

</div>