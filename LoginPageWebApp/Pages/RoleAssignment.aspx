<%@ Page Language="C#" Async="true" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="RoleAssignment.aspx.cs" Inherits="LoginPageWebApp.Pages.RoleAssignment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .role-assignment-container {
            max-width: 800px;
            margin: 40px auto;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
            padding: 32px 28px 24px 28px;
            font-family: Arial, sans-serif;
        }
        .role-assignment-container h2 {
            text-align: center;
            color: #007bff;
            margin-bottom: 24px;
        }
        .table {
            margin-top: 24px;
        }
        .btn-primary {
            min-width: 120px;
        }
        .text-danger {
            margin-bottom: 16px;
            display: block;
            text-align: center;
        }
    </style>
    <div class="role-assignment-container">
        <h2>Role Assignment</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered mt-3" OnRowDataBound="gvUsers_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Username" HeaderText="Username" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:TemplateField HeaderText="Role">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlRoles" runat="server"></asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnAssign" runat="server" Text="Assign Role"
                            CommandArgument='<%# Eval("Id") %>' OnClick="btnAssign_Click" CssClass="btn btn-primary" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
