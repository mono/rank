using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RankPage;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Retrieve_Email();
    }
	
	protected void Retrieve_Email()
    {
        Global.Email = Request.Form["email"];
    }

}
