<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchNeeded.aspx.cs" Inherits="ItProekt.WebForm4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 334px;
        }
        .style3
        {
            height: 27px;
        }
        .style4
        {
            height: 76px;
        }
        .style5
        {
            height: 31px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div>
    <br />
    &nbsp; Име/Презиме на играчот&nbsp;
    <asp:TextBox ID="TextBox1" runat="server" style="margin-bottom: 0px" 
        Width="432px"></asp:TextBox> &nbsp;
    <asp:Button ID="Button1" runat="server" Text="Барај" Width="166px" 
        onclick="Button1_Click" onclientclick="search()" />
    <br />
    <br />
    <div style="text-align: center"><asp:Label ID="Label1" runat="server" Text="Label" 
            Font-Bold="True" ForeColor="White"></asp:Label></div>
    <br />
    <br />
    </div>
    <div style="text-align: center">
    <asp:MultiView ID="MultiView1" runat="server">
    </asp:MultiView>
      <br />
    <asp:Button ID="Button2" runat="server" Text="<<" onclick="Button2_Click" 
            Visible="False" />
        <asp:Button ID="Button3"
        runat="server" Text=">>" onclick="Button3_Click" Visible="False" />
    </div>
    <br />

    <table class="style1">
        <tr>
            <td class="style2" rowspan="4">
        <asp:ListBox ID="ListBox1" runat="server" BackColor="#525100" 
            Font-Size="X-Large" ForeColor="#DADADA" Height="281px" Width="272px" 
                    AutoPostBack="True" onselectedindexchanged="ListBox1_SelectedIndexChanged"></asp:ListBox>
          
            </td>
            <td class="style5">
                <asp:Label ID="SelectedPost" runat="server" Font-Bold="True" 
                    ForeColor="White"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style4">
                <asp:Image ID="Image1" runat="server" Height="120px" style="margin-left: 19px" 
                    Width="120px" />
            </td>
        </tr>
        <tr>
            <td class="style3">
                &nbsp<asp:Label ID="PlayerInfo" runat="server" 
                    Font-Bold="True" ForeColor="White"></asp:Label>;</td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="Button4" runat="server" onclick="Button4_Click" 
                    Text="Прати порака" BorderColor="#D17F02" ForeColor="Black" Height="25px" 
                    Width="120px" />
                <asp:TextBox ID="TextBox2" runat="server" BackColor="#E77F00">Цена на вашата сликичка</asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td>
                &nbsp;<asp:Label ID="StatusMessage" runat="server" Text="Label" 
                    Font-Bold="True" ForeColor="White"></asp:Label></td>
        </tr>
    </table>
    <div style="text-align: center"><asp:Label ID="Label2" runat="server" Text="Label" 
            Font-Bold="True" ForeColor="White"></asp:Label></div>
            <script type="text/javascript">
                function search() {
                    var srch = document.getElementById("MainContent_TextBox1").value;
                    window.location.href = "SearchNeeded.aspx?search=" + srch;
                    alert();
                }
            </script>
</asp:Content>
 