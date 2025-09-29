<%@ Page Language="C#" Async="true" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="LoginPageWebApp.Pages.CreateUser" %>
<%@ Register Src="~/Controls/ValidationSummaryReusable.ascx" TagPrefix="uc" TagName="ValidationSummaryReusable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .create-user-container {
            max-width: 400px;
            margin: 40px auto;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
            padding: 32px 28px 24px 28px;
            font-family: Arial, sans-serif;
        }
        .create-user-container h2 {
            text-align: center;
            color: #007bff;
            margin-bottom: 24px;
        }
        .form-group {
            margin-bottom: 18px;
        }
        .form-group label {
            display: block;
            margin-bottom: 6px;
            color: #333;
            font-weight: 500;
        }
        .form-group input[type="text"],
        .form-group input[type="email"] {
            width: 100%;
            padding: 8px 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 15px;
        }
        .btn-primary {
            width: 100%;
            padding: 10px 0;
            background: #007bff;
            color: #fff;
            border: none;
            border-radius: 4px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: background 0.2s;
        }
        .btn-primary:hover {
            background: #0056b3;
        }
        .message-label {
            display: block;
            text-align: center;
            margin-bottom: 16px;
            font-size: 14px;
        }
    </style>
    <div class="create-user-container">
        <h2>Create User</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="message-label" ForeColor="Red" />
        <uc:ValidationSummaryReusable ID="ValidationSummary1" runat="server" />
        <div class="form-group">
            <asp:Label ID="lblUsername" runat="server" Text="Username:" AssociatedControlID="txtUsername" />
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <asp:Label ID="lblEmail" runat="server" Text="Email:" AssociatedControlID="txtEmail" />
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
        </div>
        <asp:Button ID="btnCreateUser" runat="server" Text="Create User" CssClass="btn-primary" OnClick="btnCreateUser_Click" />
    </div>
</asp:Content>