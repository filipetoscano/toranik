using System;
using System.Web.Security;

namespace WebApplication
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {
            FormsAuthentication.SignOut();

            /*
             * Don't use .RedirectToLoginPage, since it adds as ReturnUrl
             * the current page -- which would be Logout.aspx.
             */
            Response.Redirect( "~/Login.aspx" );
        }
    }
}