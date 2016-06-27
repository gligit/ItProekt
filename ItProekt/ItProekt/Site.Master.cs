using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ItProekt
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                LinkButton1.Visible = false;
            }
            if (HttpContext.Current.User.IsInRole("Members") || !  HttpContext.Current.User.Identity.IsAuthenticated)
            {
                LinkButton2.Visible = false;
            }
        
        }
    }
}
