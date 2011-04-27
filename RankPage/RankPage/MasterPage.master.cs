using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Session["name"] = Request.Form["name"].ToString();
			Session["email"] = Request.Form["email"].ToString();
        }
        catch (Exception i) { };
    }
}
