<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Manage.aspx.cs" Inherits="ItProekt.WebForm2" %>
  
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 272px;
        }
        .style3
        {
            width: 46px;
            height: 70px;
        }
        .style5
        {
            height: 70px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
    <asp:Label ID="Label2" runat="server" Text="Ова се вашите сликички" 
    Font-Size="XX-Large" ForeColor="White" Font-Bold="True"></asp:Label>
    </div>
    <br />
    <br />
    <table class="style1" style="margin-left: 30px">
        <tr>
            <td class="style2" rowspan="3">
        <asp:ListBox ID="ListBox1" runat="server" BackColor="#525100" 
            Font-Size="X-Large" ForeColor="#DADADA" Height="281px" Width="272px" 
                    AutoPostBack="True" onselectedindexchanged="ListBox1_SelectedIndexChanged"></asp:ListBox>
          
            </td>
            <td class="style3">
            </td>
            <td class="style5" colspan="4">
                <asp:Image ID="Image1" runat="server" Height="120px" style="margin-left: 19px" 
                    Width="120px" />
            </td>
            <td class="style5">
            </td>
            <td rowspan="3">
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td colspan="2">
                <asp:Label ID="PlayerInfo" runat="server" Text="Label"></asp:Label>
            </td>
            <td colspan="3">
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td>
                <asp:Button ID="Button1" runat="server" BackColor="#573A00" 
                    BorderStyle="Outset" ForeColor="White" Height="19px" Text="Постирај ја сликичката" 
                    Width="162px" onclick="Button1_Click1" />&nbsp 
                <asp:Label ID="Label3" runat="server" Text="Цена: " ForeColor="White"></asp:Label>
            &nbsp;<asp:TextBox ID="TextBox1" runat="server" BackColor="#573A00" 
                    BorderStyle="Outset" ForeColor="White" Width="76px"></asp:TextBox>
                <asp:CheckBox ID="CheckBox1" runat="server" Text="Прифаќам замена" 
                    ForeColor="White" />
                <br />
                <asp:Button ID="Button2" runat="server" BackColor="#573A00" 
                    BorderStyle="Outset" ForeColor="White" Height="19px" Text="Уништи ја сликичката" 
                    Width="162px" onclick="Button2_Click1" />
            </td>
            <td colspan="2">
                &nbsp;</td>
            <td colspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td colspan="6" align="center">
                <asp:Label ID="Label1" runat="server" Text="Label1" ForeColor="White"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td colspan="6">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <hr/>
    <br />
    <div style="text-align: center">
        <asp:Label ID="Label4" runat="server" 
            Text="Внеси го името/презимето на играчот и стисни го копчето за да поставиш оглас" 
            Font-Size="XX-Large" ForeColor="White" Font-Italic="True"></asp:Label><br /><br />
            <asp:TextBox ID="TextBox2" runat="server" BackColor="#573A00" 
                    BorderStyle="Outset" ForeColor="White" Width="234px" Height="36px" 
            style="margin-bottom: 12px"></asp:TextBox>
                <br />    <br />
                            <asp:Button ID="Button3" runat="server" BackColor="#573A00" 
                    BorderStyle="Outset" ForeColor="White" Height="25px" Text="Постави оглас" 
                    Width="234px" Font-Size="Large" onclick="Button3_Click" />
    </div>
        <br />
    <div style="text-align:center">
        <asp:Label ID="Label5" runat="server" Text="Label" ForeColor="White"></asp:Label>
    </div>
     <br />
    <div style="text-align: center">
        <asp:Label ID="Label6" runat="server" 
            Text="Вашите огласи" 
            Font-Size="XX-Large" ForeColor="White" Font-Bold="True"></asp:Label>
        <br />
        <br />
        
        <asp:ListBox ID="ListBox2" runat="server" BackColor="#525100" 
            ForeColor="#DADADA" Height="166px" Width="468px"></asp:ListBox>
                <br />
            <asp:Button ID="Button4" runat="server" BackColor="#573A00" 
                    BorderStyle="Outset" ForeColor="White" Height="25px" Text="Избриши оглас" 
                    Width="468px" Font-Size="Large" onclick="Button4_Click" />
                    <br /><br /><br /><br />
    </div>
</asp:Content>
 