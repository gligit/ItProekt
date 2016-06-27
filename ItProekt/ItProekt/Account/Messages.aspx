<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Messages.aspx.cs" Inherits="ItProekt.WebForm5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
<div style="text-align:center;">
    <asp:ListBox ID="ListBox1" runat="server" Height="276px" Width="556px" 
        AutoPostBack="True" onselectedindexchanged="ListBox1_SelectedIndexChanged"></asp:ListBox>
    <br /> <br /> 
    <table class="style1">
        <tr>
            <td width="50%">
                <asp:Image ID="Image1" runat="server" Height="120px" Width="120px" 
                    Visible="False" />
            </td>
            <td width="50%">
                <asp:Image ID="Image2" runat="server" Height="120px" Width="120px" 
                    Visible="False" />
            </td>
        </tr>
        <tr>
            <td>
&nbsp;<asp:Label ID="Label2" runat="server" Text="Вашата сликичка" ForeColor="White" 
                    Font-Bold="True" Visible="False"></asp:Label>
            </td>
            <td>
&nbsp;<asp:Label ID="Label3" runat="server" Text="Сликичката на праќачот" ForeColor="White" 
                    Font-Bold="True" Visible="False"></asp:Label>
            </td>
        </tr>
    </table>
    <br />

    <asp:Label ID="Label1" runat="server" ForeColor="White" Font-Bold="True"></asp:Label>&nbsp;<br /><br />
    <asp:Button ID="Button1" runat="server" Text="Прифати" Height="25px" 
        Width="120px" onclick="Button1_Click" BorderColor="#D17F02" 
        Visible="False" />&nbsp;
    <asp:Button ID="Button2"
        runat="server" Text="Одбиј" Height="25px" Width="120px" 
        onclick="Button2_Click" BorderColor="#D17F02" Visible="False" />
    <br /><br />
    <asp:Label ID="StatusMessage" runat="server" Text="Label" ForeColor="White" 
        Font-Bold="True"></asp:Label>
</div>
</asp:Content>
