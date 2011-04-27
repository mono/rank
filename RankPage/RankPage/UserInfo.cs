using System;

public class UserInfo
{
	public string Name {get; set;}
    public string Email { get; set; }
    public string Gravatar { get; set; }
	
	public UserInfo(string name, string email)
	{
		Name = name;
		Email = email;
	}
	
//	public struct UserData
//	{
//		public string name {get; set;}
//		public string email {get; set;}
//	}
}

