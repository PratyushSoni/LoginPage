<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="LoginPageWebApp.Pages.Home" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>Home</title>
    <style>
        body {
            font-family: Arial;
            background: #f5f6fa;
            margin: 0;
            padding: 0;
        }

        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 20px;
            background: #007bff;
            color: #fff;
        }

        .user-menu {
            position: relative;
            display: inline-block;
        }

        .dropdown {
            display: none;
            position: absolute;
            right: 0;
            background-color: #fff;
            min-width: 150px;
            box-shadow: 0px 8px 16px rgba(0,0,0,0.2);
            border-radius: 4px;
        }

        .dropdown a {
            color: #000;
            padding: 10px;
            display: block;
            text-decoration: none;
        }

        .dropdown a:hover {
            background: #f1f1f1;
        }

        .user-menu:hover .dropdown {
            display: block;
        }

        .content {
            padding: 20px;
        }

        .btn {
            padding: 8px 16px;
            background: #007bff;
            color: #fff;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }

        .btn:hover {
            opacity: 0.9;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h2>Home</h2>
            <div class="user-menu">
                <span><asp:Label ID="lblUser" runat="server" /></span>
                <div class="dropdown">
                    <asp:LinkButton ID="lnkSignOut" runat="server" OnClick="lnkSignOut_Click">Sign Out</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="content">
            <asp:Label ID="lblWelcome" runat="server" Font-Size="Large" />

            <br /><br />
            <asp:Button ID="btnCreateUser" runat="server" Text="Create User" CssClass="btn" Visible="false" />
        </div>
    </form>
</body>
</html>
