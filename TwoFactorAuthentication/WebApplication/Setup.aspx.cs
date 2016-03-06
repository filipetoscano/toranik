using System;
using WebApplication.Controllers;

namespace WebApplication
{
    public partial class Setup : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {
            TwoFactorController c = new TwoFactorController();
            var r = c.SetupGet();

            UserId.Text = r.UserId;
            TfSetup.ImageUrl = r.ImageUrl;
            ManualCode.Text = r.ManualCode;
            DataBind();
        }
    }
}