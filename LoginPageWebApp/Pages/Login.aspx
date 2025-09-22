<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LoginPageWebApp.Pages.Login" %>

<%@ Register Src="~/Controls/ValidationSummaryReusable.ascx" TagPrefix="uc1" TagName="ValidationSummaryReusable" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>Login</title>
    <style>
        html, body {
            height: 100%;
            margin: 0;
            font-family: Arial;
            background: #f5f6fa;
        }

        .login-container {
            display: flex;
            align-items: center;
            justify-content: center;
            height: 100vh;
        }

        .login-box {
            width: 360px;
            padding: 28px;
            background: #fff;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            border-radius: 8px;
        }

            .login-box h2 {
                text-align: center;
                margin-bottom: 18px;
            }

        .form-group {
            margin-bottom: 14px;
        }

            .form-group label {
                display: block;
                margin-bottom: 6px;
                font-size: 14px;
            }

        .text-input {
            width: 100%;
            padding: 10px;
            font-size: 14px;
            box-sizing: border-box;
            border: 1px solid #ddd;
            border-radius: 4px;
        }

        .form-actions {
            display: flex;
            justify-content: flex-end;
            margin-top: 10px;
        }

        .btn {
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-primary {
            background: #007bff;
            color: #fff;
        }

            .btn-primary:hover {
                opacity: 0.95;
            }

        .text-danger {
            color: #d9534f;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-box">
                <h2>Sign in</h2>
                <div class="val-summary">
                    <uc1:ValidationSummaryReusable ID="ValidationSummary1" runat="server" />
                </div>
                <div class="form-group">
                    <label for="<%= txtUsername.ClientID %>">Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                        ErrorMessage="Username is required" Display="None" />
                </div>

                <div class="form-group">
                    <label for="<%= txtPassword.ClientID %>">Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                        ErrorMessage="Password is required" Display="None" />
                </div>


                <div class="form-actions">
                    <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" OnClick="btnLogin_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
