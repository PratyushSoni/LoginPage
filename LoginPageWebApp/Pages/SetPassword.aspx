<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="SetPassword.aspx.cs" Inherits="LoginPageWebApp.Pages.SetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Set Password</title>
    <style>
        .set-password-container {
            max-width: 400px;
            margin: 40px auto;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
            padding: 32px 28px 24px 28px;
            font-family: Arial, sans-serif;
        }
        .set-password-container h2 {
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
        .form-group input[type="password"] {
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
</head>
<body>
    <form id="form1" runat="server">
        <div class="set-password-container">
            <h2>Set Your Password</h2>
            <asp:Label ID="lblMessage" runat="server" CssClass="message-label" ForeColor="Red" />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="message-label" ShowMessageBox="false" ShowSummary="true" />
            <div class="form-group">
                <asp:Label ID="lblPassword" runat="server" Text="Password:" AssociatedControlID="txtPassword" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>
            <div class="form-group">
                <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password:" AssociatedControlID="txtConfirmPassword" />
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>
            <asp:Button ID="btnSetPassword" runat="server" Text="Set Password" CssClass="btn-primary" OnClick="btnSetPassword_Click" />
        </div>
    </form>
</body>
</html>
