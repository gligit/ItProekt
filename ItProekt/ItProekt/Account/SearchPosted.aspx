<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchPosted.aspx.cs" Inherits="ItProekt.WebForm3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            height: 209px;
        }
        .style2
        {
            height: 202px;
        }
        .style4
        {
            width: 365px;
            height: 7px;
        }
        .style5
        {
            height: 7px;
            width: 643px;
        }
        .style8
        {
            width: 643px;
        }
    </style>
         <script type ="text/jscript">
             function search() {


                 var srch = document.getElementById("MainContent_TextBox1").value;
                 window.location.href = "SearchPosted.aspx?search=" + srch;
                 alert();
             }
     </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
    <br />
    &nbsp; Име/Презиме на играчот&nbsp;
    <asp:TextBox ID="TextBox1" runat="server" style="margin-bottom: 0px" 
        Width="432px"></asp:TextBox> &nbsp;
    <asp:Button ID="Button1" runat="server" Text="Барај" Width="166px" 
            onclientclick="search()" />
    <br />
    <br />
    <div style="text-align: center">
    <asp:Label ID="Label1" runat="server" Text="Label" Font-Bold="True" 
            ForeColor="White"></asp:Label>
            </div>
    <br />
  
    </div>
    <asp:MultiView ID="MultiView1" runat="server">
    </asp:MultiView>
    <div style="text-align: center">
    <asp:Button ID="Button5" runat="server" Text="<<" onclick="Button5_Click" 
            Visible="False" />
        <asp:Button ID="Button6"
        runat="server" Text=">>" onclick="Button6_Click" Visible="False" />
        </div>
    <div>
    <br />
    <br />
    <table class="style1" style="margin-left: 30px">
        <tr>
            <td class="style2" rowspan="4">
        <asp:ListBox ID="ListBox1" runat="server" BackColor="#525100" 
            Font-Size="X-Large" ForeColor="#DADADA" Height="281px" Width="194px" 
                    AutoPostBack="True" onselectedindexchanged="ListBox1_SelectedIndexChanged"></asp:ListBox>
          
            </td>
            <td class="style5" align="center">
                <asp:Label ID="Label6" runat="server" Font-Size="Medium" ForeColor="White" 
                    Text="Вашата сликичка" Font-Bold="True"></asp:Label>
            </td>
            <td class="style4" align="center">
                <asp:Label ID="Label5" runat="server" Font-Size="Medium" ForeColor="White" 
                    Text="Селектираната сликичка" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style5" align="center">
                <asp:Image ID="Image1" runat="server" Height="120px"  
                    Width="120px" />
            </td>
            <td class="style4" align="center">
                <asp:Image ID="Image2" runat="server" Height="120px" Width="120px" />
            </td>
        </tr>
        <tr>
            <td align="center" class="style8">
                <asp:Label ID="PlayerInfo" runat="server" Text="Label" Font-Bold="True" 
                    ForeColor="White"></asp:Label>
            </td>
            <td align="center">
                <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="White" 
                    Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center" class="style8">
                &nbsp 
                &nbsp;<asp:Button ID="Button4" runat="server" onclick="Button4_Click" 
                    Text="Деселектирај" BorderColor="#D17F02" ForeColor="Black" Height="25px" 
                    Width="120px" />
                <br />
            </td>
            <td>
                <asp:Button ID="Button3" runat="server" onclick="Button3_Click" Text="Прати порака" 
                    BorderColor="#D17F02" ForeColor="Black" Height="25px" Width="120px" />
                <asp:TextBox ID="TextBox3" runat="server" BackColor="#E77F00" ForeColor="White">Вашата понуда</asp:TextBox>
            </td>
        </tr>
        </table>
         <br />
         <div style="text-align: center">
        <asp:Label ID="Label2" runat="server" Text="Label" ForeColor="White" 
            Font-Bold="True"></asp:Label>
            </div>
        <br />
    </div>
    <br/> <br/> <br/> <br/>

</asp:Content>
