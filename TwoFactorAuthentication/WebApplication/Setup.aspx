<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setup.aspx.cs" Inherits="WebApplication.Setup" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TFA / Setup</title>
    <link rel="stylesheet" type="text/css" href="css/main.css" />
</head>
<body>
    <form id="form1" runat="server">
        <nav>
            <a href="Default.aspx">Validate</a> |
            Setup |
            <a href="Logout.aspx">Logout</a>
        </nav>

        <h1>Setup</h1>

        <div>
            User Id: <br />
            <asp:TextBox ID="UserId" runat="server" ReadOnly="true" />
        </div>

        <div>
            Manual code: <br />
            <asp:TextBox ID="ManualCode" runat="server" ReadOnly="true" />
        </div>

        <div>
            <asp:Image ID="TfSetup" runat="server" />
        </div>
    </form>
</body>
</html>
