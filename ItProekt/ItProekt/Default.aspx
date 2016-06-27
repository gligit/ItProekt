<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="ItProekt._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>Апликација за размена на сликички од албуми за светско првенство<br /><br />&nbsp;<asp:HyperLink 
            ID="HyperLink1" runat="server" NavigateUrl="~/Account/Register.aspx">Регистрирајте се</asp:HyperLink>
&nbsp;за да можете да пристапите до другите страници </h2>
</asp:Content>
 