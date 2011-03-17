using System;

class UserInfo
{
	protected string Name {get; set;}
	protected string Email {get; set;}
	protected string Gravatar {get; set;}
	
	public UserInfo(string name, string email)
	{
		Name = name;
		Email = email;
	}
}

