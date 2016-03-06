<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication.Login" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TFA / Login</title>
    <link rel="stylesheet" type="text/css" href="css/main.css" />
</head>
<body>
    <form id="form1" runat="server">
        <h1>Login</h1>
    
        <asp:TextBox ID="UserId" runat="server" />
        <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" />

    </form>
</body>
</html>
