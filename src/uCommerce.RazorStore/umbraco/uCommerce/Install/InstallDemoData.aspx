<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstallDemoData.aspx.cs" Inherits="uCommerce.RazorStore.Umbraco.ucommerce.Install.InstallDemoData" %>
<%@ Register tagPrefix="uc" tagName="Installer" src="DemoStoreInstaller.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <uc:Installer runat="server" ID="installer" />
    </form>
</body>
</html>
