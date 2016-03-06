<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TFA / Validate</title>
    <link rel="stylesheet" type="text/css" href="css/main.css" />
</head>
<body>
    <script src="js/jquery.min.js"></script>

    <form id="form1" runat="server" autocomplete="off">
        <nav>
            Validate |
            <a href="Setup.aspx">Setup</a> |
            <a href="Logout.aspx">Logout</a>
        </nav>

        <h1>Validate</h1>

        <input id="ValidateCode" type="text" maxlength="6" />
        <button id="btnValidate" value="1">Validate</button>
        <input id="IsValid" type="text" maxlength="10" readonly="true" style="width:40px" />


        <script>

            $('#ValidateCode').keypress(function (event) {
                if (event.which == 13) {
                    $('#btnValidate').click();
                }
            });

            $('#btnValidate').click(function (event) {
                $('#IsValid').val('');

                var req = {
                    Code: $('#ValidateCode').val()
                };

                $.post('/api/TwoFactor/Validate', req, function (data) {
                    var text;
                    var css;

                    if ( data.IsValid == true ) {
                        text = 'OK';
                        css = 'ok';
                    }
                    else {
                        text = 'Invalid';
                        css = 'nok'
                    }

                    $('#IsValid')
                        .val(text)
                        .removeClass('ok nok')
                        .addClass(css);
                });

                event.preventDefault();
                return false;
            });

        </script>
    </form>
</body>
</html>
