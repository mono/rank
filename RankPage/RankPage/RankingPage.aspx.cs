using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Global;

public partial class RankingPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		currentUserName.Text = Global.Global.Name;
    }
	
    protected void btnShowHow_Click(object sender, EventArgs e)
    {

    }
}