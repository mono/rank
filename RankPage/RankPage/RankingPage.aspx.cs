using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RankingPage : System.Web.UI.Page
{
	private UserInfo user;
	
    protected void Page_Load(object sender, EventArgs e)
    {
		//user = new UserInfo(Request.Form["name"].ToString(), Request.Form["email"].ToString());
    }
	
    protected void btnShowHow_Click(object sender, EventArgs e)
    {

    }
}