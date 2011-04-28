using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gravatarlib;
using Rankdblib;

public partial class RankingPage : System.Web.UI.Page
{
	public UserInfo user;
	
    protected void Page_Load(object sender, EventArgs e)
    {
		string name = "";
		string email = "";
		try
		{
			name = (string)Session["name"];
			email = (string)Session["email"];

		}
		catch (Exception i){};
		
		currentUserName.Text = name;
		currentUserGravatar.ImageUrl = new Gravatar(email, Gravatar.IconSets.identicon, Gravatar.Ratings.g, 50).GravatarURL();
		
		List<Users> users;
		Database db = new Database();
		users = db.RetrieveUsers(); //This needs to be stored in an html5 browser db.

		foreach (Users user in users)
		{
			if (user.email.Contains(email))
			{
				currentUserScore.Text = user.Score().ToString();
				Session["currentUser"] = user;
			}
		}
    }
}