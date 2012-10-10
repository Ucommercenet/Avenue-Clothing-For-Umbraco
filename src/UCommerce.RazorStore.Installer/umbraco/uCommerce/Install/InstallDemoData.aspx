<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstallDemoData.aspx.cs" Inherits="uCommerce.RazorStore.Umbraco.ucommerce.Install.InstallDemoData" %>
<%@ Register TagPrefix="uc" TagName="Installer" Src="DemoStoreInstaller.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/DependencyHandler.axd?s=L3VtYnJhY29fY2xpZW50L3VpL2RlZmF1bHQuY3NzOy91bWJyYWNvX2NsaWVudC9tb2RhbC9zdHlsZS5jc3M7L3VtYnJhY29fY2xpZW50L3Byb3BlcnR5cGFuZS9zdHlsZS5jc3M7L3VtYnJhY29fY2xpZW50L3Njcm9sbGluZ21lbnUvc3R5bGUuY3NzOy91bWJyYWNvX2NsaWVudC9wYW5lbC9zdHlsZS5jc3M7&amp;t=Css&amp;cdv=1" type="text/css" rel="stylesheet">
    <script src="/DependencyHandler.axd?s=L3VtYnJhY29fY2xpZW50L0FwcGxpY2F0aW9uL05hbWVzcGFjZU1hbmFnZXIuanM7L3VtYnJhY29fY2xpZW50L3VpL2pxdWVyeS5qczsvdW1icmFjb19jbGllbnQvdWkvanF1ZXJ5dWkuanM7L3VtYnJhY29fY2xpZW50L0FwcGxpY2F0aW9uL2pRdWVyeS9qcXVlcnkuY29va2llLmpzOy91bWJyYWNvX2NsaWVudC9BcHBsaWNhdGlvbi9VbWJyYWNvQXBwbGljYXRpb25BY3Rpb25zLmpzOy91bWJyYWNvX2NsaWVudC9BcHBsaWNhdGlvbi9VbWJyYWNvVXRpbHMuanM7L3VtYnJhY29fY2xpZW50L0FwcGxpY2F0aW9uL1VtYnJhY29DbGllbnRNYW5hZ2VyLmpzOy91bWJyYWNvX2NsaWVudC9tb2RhbC9tb2RhbC5qczsvdW1icmFjb19jbGllbnQvdWkvZGVmYXVsdC5qczsvdW1icmFjb19jbGllbnQvQXBwbGljYXRpb24valF1ZXJ5L2pxdWVyeS5ob3RrZXlzLmpzOy91bWJyYWNvX2NsaWVudC9zY3JvbGxpbmdtZW51L2phdmFzY3JpcHQuanM7L3VtYnJhY29fY2xpZW50L3BhbmVsL2phdmFzY3JpcHQuanM7&amp;t=Javascript&amp;cdv=1" type="text/javascript"></script>
</head>
<body class="umbracoPage" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
    <div id="body_Panel1" class="panel" style="height: 883px; width: 1254px;">
        <div class="boxhead">
            <h2 id="body_Panel1Label">Install package</h2>
        </div>
        <div class="boxbody">
            <div id="body_Panel1_content" class="content" style="width: 1248px; height: 837px;">
                <div class="innerContent" id="body_Panel1_innerContent" style="">
                    <div class="propertypane" style="">
                        <div>
                            <uc:Installer runat="server" ID="installer" />
                            <div class="propertyPaneFooter">
                                -</div>
                        </div>
                    </div>
                    <input name="ctl00$body$tempFile" type="hidden" id="body_tempFile"><input name="ctl00$body$processState" type="hidden" id="body_processState">
                </div>
            </div>
        </div>
        <div class="boxfooter">
            <div class="statusBar">
                <h2></h2>
            </div>
        </div>
    </div>
    <script type="text/javascript">
//<![CDATA[
        jQuery(document).ready(function () { jQuery(window).load(function () { resizePanel('body_Panel1', false, true); }) });//]]>
    </script>
    </form>
</body>
</html>
