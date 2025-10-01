<%@ Page Language="C#" MasterPageFile="~/Site.master" Async="true" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="LoginPageWebApp.Pages.CreateUser" %>

<%@ Register Src="~/Controls/ValidationSummaryReusable.ascx" TagPrefix="uc" TagName="ValidationSummaryReusable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        html, body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f5f6fa;
        }

        .create-user-container {
            max-width: 400px;
            margin: 60px auto;
            padding: 30px 28px;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
        }

            .create-user-container h2 {
                text-align: center;
                color: #007bff;
                margin-bottom: 24px;
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

        .btn-primary {
            width: 100%;
            padding: 10px 0;
            background-color: #007bff;
            color: #fff;
            border: none;
            border-radius: 4px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: opacity 0.2s;
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

        .message-label {
            display: block;
            font-size: 13px;
            margin-bottom: 12px;
        }
    </style>

    <div class="create-user-container">
        <h2>
            <asp:Literal runat="server" Text="<%$ Resources:SharedResource, CreateUser_Heading %>" />
        </h2>

        <!-- Validation summary for errors -->
        <uc:ValidationSummaryReusable ID="ValidationSummary1" runat="server" />

        <!-- Success/Error messages -->
        <asp:Label ID="lblMessage" runat="server" CssClass="message-label" />

        <div class="form-group">
            <label>
                <asp:Literal runat="server" Text="<%$ Resources:SharedResource, CreateUser_Username %>" />
            </label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="text-input" TabIndex="1" />
        </div>

        <div class="form-group">
            <label>
                <asp:Literal runat="server" Text="<%$ Resources:SharedResource, CreateUser_Email %>" />
            </label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="text-input" TextMode="Email" TabIndex="2" />
        </div>

        <asp:Button ID="btnCreateUser" runat="server" CssClass="btn btn-primary" OnClick="btnCreateUser_Click" Text="<%$ Resources:SharedResource, CreateUser_Button %>" />
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            var summary = document.querySelector('.validation-summary-container');
            if (summary) {
                summary.addEventListener('click', function (e) {
                    if (e.target && e.target.nodeName === 'LI') {
                        var text = e.target.textContent || e.target.innerText;
                        if (text && text.toLowerCase().indexOf('username') !== -1) {
                            var usernameInput = document.getElementById('<%= txtUsername.ClientID %>');
                            if (usernameInput) usernameInput.focus();
                        } else if (text && text.toLowerCase().indexOf('email') !== -1) {
                            var emailInput = document.getElementById('<%= txtEmail.ClientID %>');
                            if (emailInput) emailInput.focus();
                        }
                    }
                });
            }
        });
    </script>

</asp:Content>
