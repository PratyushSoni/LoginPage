<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="LoginPageWebApp.Pages.AccessDenied" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="margin:50px;">
        <h2>Access Denied</h2>
        <p>You do not have permission to view this page.</p>
        <a href="Login.aspx">Go back</a>
    </div>
</asp:Content>
