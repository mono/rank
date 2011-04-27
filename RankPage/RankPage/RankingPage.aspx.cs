using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gravatarlib;

public partial class RankingPage : System.Web.UI.Page
{
	public UserInfo user;
	
    protected void Page_Load(object sender, EventArgs e)
    {
		try
		{
			currentUserName.Text = (string)Session["name"];
			Gravatar gravatar = new Gravatar((string)Session["email"], Gravatar.IconSets.identicon, Gravatar.Ratings.g, 50);
			currentUserGravatar.ImageUrl = gravatar.GravatarURL();
		}
		catch (Exception i){};
    }
	
    protected void btnShowHow_Click(object sender, EventArgs e)
    {

    }
}