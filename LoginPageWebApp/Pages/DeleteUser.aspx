<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Async="true" CodeBehind="DeleteUser.aspx.cs" Inherits="LoginPageWebApp.Pages.DeleteUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .delete-user-container {
            max-width: 700px;
            margin: 60px auto;
            padding: 30px 28px;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
        }

        .user-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

            .user-table th, .user-table td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }

            .user-table th {
                background: #f5f6fa;
            }

        .btn-danger {
            background: #dc3545;
            color: #fff;
            border: none;
            border-radius: 4px;
            padding: 6px 12px;
            cursor: pointer;
            font-size: 14px;
        }

            .btn-danger:hover {
                opacity: 0.95;
            }

        .message-label {
            display: block;
            font-size: 13px;
            margin-bottom: 12px;
        }
    </style>
    <div class="delete-user-container">
        <h2>Delete Users</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="message-label" />
        <asp:Repeater ID="rptUsers" runat="server">
            <HeaderTemplate>
                <table class="user-table">
                    <tr>
                        <th>Username</th>
                        <th>Email</th>
                        <th>Roles</th>
                        <th>Action</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Username") %></td>
                    <td><%# Eval("Email") %></td>
                    <td><%# string.Join(", ", (System.Collections.Generic.IEnumerable<string>)Eval("Roles")) %></td>
                    <td>
                        <asp:Button runat="server" CommandName="DeleteUser" CommandArgument='<%# Eval("Username") %>' Text="Delete" CssClass="btn-danger" OnCommand="DeleteUser_Command" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
