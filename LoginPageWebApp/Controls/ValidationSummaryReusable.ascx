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

    .summary-header {
        font-weight: bold;
        margin-bottom: 5px;
    }
</style>

<asp:Panel ID="pnlValidationSummary" runat="server" CssClass="validation-summary-container" Visible="false">
    <asp:Label ID="lblHeader" runat="server" Text="<%$ Resources:SharedResource, ValidationSummaryHeader %>" CssClass="summary-header" />
    <ul id="ulValidationSummary" runat="server"></ul>
    <asp:CustomValidator ID="cvServerError" runat="server" Display="None" EnableClientScript="false" />
</asp:Panel>

<script type="text/javascript">
document.addEventListener('DOMContentLoaded', function () {
    var ul = document.getElementById('<%= ulValidationSummary.ClientID %>');
    if (!ul) return;

    ul.addEventListener('click', function(e) {
        if (e.target.tagName === 'LI') {
            var targetId = e.target.getAttribute('data-target');
            if (targetId) {
                var ctrl = document.getElementById(targetId);
                if (ctrl) ctrl.focus();
            }
        }
    });
});
</script>
