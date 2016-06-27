<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MakeStickers.aspx.cs" Inherits="ItProekt.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
    <div style="padding: 0px 10px 0px 10px; border-style: solid; margin: 20px 0px 20px 0px;">
         <h2 style="text-align: center">&nbsp;Креирај сликичка&nbsp;
             <asp:FileUpload ID="FileUpload1" runat="server" accept=".png,.jpg,.jpeg,.gif"/>Внеси 
             Име и Презиме на играчот
             <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
&nbsp;и<br /></h2>
   
   <h3>Подари ја на некој корисник. Име на корисник <asp:TextBox ID="TextBox1" 
           runat="server" Width="120px"></asp:TextBox>&nbsp;<asp:Button 
           ID="Button1" runat="server" Text="Креирај" Width="112px" 
           onclick="Button1_Click" /><br/>или<br/>
       Постави ја за продавање. Цена на сликичката&nbsp; 
           <asp:TextBox ID="TextBox3" runat="server" style="margin-left: 0px" 
           Width="123px"></asp:TextBox>&nbsp;<asp:Button 
          ID="Button3" runat="server" Text="Креирај" Width="112px" 
           onclick="Button3_Click" />
       </h3> 
           <div style="text-align: center">Сликичката треба да биде со димензии 120x120,ако е 
               поголема/помала серверот ќе ја намали/зголеми<br />
               <asp:Label ID="Label1" runat="server" Text="Label" 
                   Font-Size="Large" ForeColor="White"></asp:Label></div>
    </div>
    </asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
    <asp:LoginView ID="HeadLoginView" runat="server" EnableViewState="false">
                    <AnonymousTemplate>
                        [ <a href="~/Account/Login.aspx" ID="HeadLoginStatus0" runat="server" 
                            style="color: #FFFFFF">Log In</a> ]
                    </AnonymousTemplate>
                    <LoggedInTemplate>
                        Добредојде <asp:LoginName ID="HeadLoginName" runat="server" />!
                        [ <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="Log Out" LogoutPageUrl="~/" ForeColor="White" /> ]
                    </LoggedInTemplate>
                </asp:LoginView>
    </asp:Content>
