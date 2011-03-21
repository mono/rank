using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RankingPage : System.Web.UI.Page
{
	public UserInfo user;
	
    protected void Page_Load(object sender, EventArgs e)
    {
        
        //string name = Request.Form["userform"]["name"].Value.ToString();
        //Page.PreviousPage.Request
        //string name = Master.FindControl("name").ToString();
        user = new UserInfo(Request.Form["name"], Request.Form["email"]);
		currentUserName.Text = user.Name;
    }
	
    protected void btnShowHow_Click(object sender, EventArgs e)
    {

    }
}