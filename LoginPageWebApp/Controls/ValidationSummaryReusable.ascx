<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ValidationSummaryReusable.ascx.cs" Inherits="LoginPageWebApp.Controls.ValidationSummaryReusable" %>

<style type="text/css">
    .validation-summary-container {
        margin-bottom: 10px;
        color: #d9534f;
        font-size: 13px;
    }

        .validation-summary-container ul {
            list-style: disc;
            margin-left: 20px;
            padding-left: 0;
        }

        .validation-summary-container li {
            cursor: pointer;
            margin-bottom: 3px;
            text-decoration: underline;
        }
</style>

<asp:Panel ID="pnlValidationSummary" runat="server" CssClass="validation-summary-container" Visible="true">
    <asp:Label ID="lblHeader" runat="server" Text="Please fix the following errors:" CssClass="summary-header" />
    <ul id="ulValidationSummary" runat="server"></ul>
    <asp:CustomValidator ID="cvServerError" runat="server" Display="None" EnableClientScript="false" />
</asp:Panel>

<script type="text/javascript">
    document.addEventListener('DOMContentLoaded', function () {
        var ul = document.getElementById('<%= ulValidationSummary.ClientID %>');
        var controlIds = '<%= TargetControlClientID %>'.split(',');
        var usernameInputId = controlIds[0];
        var emailInputId = controlIds.length > 1 ? controlIds[1] : null;
        if (ul) {
            ul.addEventListener('click', function (e) {
                var li = e.target;
                if (li.tagName === 'LI') {
                    var text = li.textContent || li.innerText || '';
                    if (text.toLowerCase().indexOf('email') !== -1 && emailInputId) {
                        var emailInput = document.getElementById(emailInputId);
                        if (emailInput) {
                            emailInput.focus();
                            emailInput.scrollIntoView({ behavior: "smooth", block: "center" });
                        }
                    } else if (usernameInputId) {
                        var usernameInput = document.getElementById(usernameInputId);
                        if (usernameInput) {
                            usernameInput.focus();
                            usernameInput.scrollIntoView({ behavior: "smooth", block: "center" });
                        }
                    }
                }
            });
        }
    });
</script>
