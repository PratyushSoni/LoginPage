<%@ Page Language="C#" MasterPageFile="~/Site.master" Async="true" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LoginPageWebApp.Pages.Login" %>
<%@ Register Src="~/Controls/ValidationSummaryReusable.ascx" TagPrefix="uc1" TagName="ValidationSummaryReusable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        html, body {
            height: 100%;
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f5f6fa;
        }

        .login-container {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .login-box {
            width: 360px;
            background: #fff;
            padding: 30px 28px;
            border-radius: 8px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
        }

        .login-box h2 {
            text-align: center;
            color: #007bff;
            margin-bottom: 20px;
            font-size: 24px;
        }

        .form-group {
            margin-bottom: 16px;
        }

        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 500;
            color: #333;
        }

        .text-input {
            width: 100%;
            padding: 10px;
            font-size: 14px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
        }

        .form-actions {
            margin-top: 18px;
            display: flex;
            justify-content: center;
        }

        .btn {
            padding: 10px 16px;
            font-size: 14px;
            font-weight: 600;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }

        .btn-primary {
            background-color: #007bff;
            color: #fff;
            width: 100%;
        }

        .btn-primary:hover {
            opacity: 0.95;
        }

        .validation-summary-container {
            margin-bottom: 12px;
            font-size: 13px;
            color: #d9534f;
        }

        .validation-summary-container ul {
            list-style: disc;
            padding-left: 20px;
            margin: 0;
        }

        .validation-summary-container li {
            cursor: pointer;
            margin-bottom: 4px;
            text-decoration: underline;
        }
    </style>

    <div class="login-container">
        <div class="login-box">
            <h2>
                <asp:Literal runat="server" Text="<%$ Resources:SharedResource, SignIn %>" />
            </h2>

            <uc1:ValidationSummaryReusable ID="ValidationSummary1" runat="server" />

            <div class="form-group">
                <label for="txtUsername">
                    <asp:Literal runat="server" Text="<%$ Resources:SharedResource, Username %>" />
                </label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="text-input" />
            </div>

            <div class="form-group">
                <label for="txtPassword">
                    <asp:Literal runat="server" Text="<%$ Resources:SharedResource, Password %>" />
                </label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="text-input" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary" OnClick="btnLogin_Click" Text="<%$ Resources:SharedResource, SignIn %>" />
            </div>
        </div>
    </div>
</asp:Content>
