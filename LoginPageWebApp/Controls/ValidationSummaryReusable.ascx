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
</asp:Panel>


<script type="text/javascript">
    function buildValidationSummary() {
        var ul = document.getElementById('<%= ulValidationSummary.ClientID %>');
        if (!ul) return;
        ul.innerHTML = "";

        if (typeof (Page_Validators) !== "undefined") {
            for (var i = 0; i < Page_Validators.length; i++) {
                var v = Page_Validators[i];
                if (!v.isvalid && v.errormessage) {
                    var ctlId = v.controltovalidate;

                    var li = document.createElement("li");
                    li.textContent = v.errormessage;
                    li.setAttribute("data-target", ctlId);

                    li.onclick = function () {
                        var target = this.getAttribute("data-target");
                        var ctl = document.getElementById(target);
                        if (ctl) {
                            ctl.focus();
                            ctl.scrollIntoView({ behavior: "smooth", block: "center" });
                        }
                    };

                    ul.appendChild(li);
                }
            }
        }

        var panel = document.getElementById('<%= pnlValidationSummary.ClientID %>');
        if (panel) {
            panel.style.display = ul.children.length > 0 ? 'block' : 'none';
        }
    }

    // Run once on page load
    window.addEventListener("load", buildValidationSummary);

    // Run after each validation cycle
    if (typeof (Page_ClientValidate) !== "undefined") {
        var oldValidate = Page_ClientValidate;
        Page_ClientValidate = function (validationGroup) {
            var result = oldValidate(validationGroup);
            setTimeout(buildValidationSummary, 50);
            return result;
        };
    }

    // Run after async postbacks too (UpdatePanel support)
    if (typeof (Sys) !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            buildValidationSummary();
        });
    }
</script>
