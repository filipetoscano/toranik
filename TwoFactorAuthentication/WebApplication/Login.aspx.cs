using System;
using System.Web.Security;

namespace WebApplication
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {
        }

        protected void btnLogin_Click( object sender, EventArgs e )
        {
            FormsAuthentication.RedirectFromLoginPage( UserId.Text, false );
        }
    }
}