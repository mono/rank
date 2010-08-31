using System;
using System.IO;
using System.Reflection;

namespace getRank
{
	public class HtmlOut
	{
		private static string dir = getDir();
		internal static string header = "";
		internal static string user = "";
		internal static string footer = "";
		
		private static string getDir()
		{
			return Directory.GetCurrentDirectory() + "/htdocs/";
		}
		
		internal static void Template()
		{
			StreamReader read = File.OpenText(dir + "template.html");
			string template = "";
			while (!read.EndOfStream)
			{
				template += read.ReadLine() + Environment.NewLine;
			}
			int front_user = template.IndexOf("<!-- Begin User -->");
			int aft_front_user = front_user + 19;
			int end_user = template.IndexOf("<!-- End User -->");
			int aft_end_user = end_user + 17;
			header = template.Substring(0, front_user);
			user = template.Substring(front_user + 19, end_user - aft_front_user);
			footer = template.Substring(aft_end_user, template.Length - aft_end_user);
		}
		
		internal static string UserRank(string name, string email, int rank, int code)
		{
			string userData = user.Replace("<!-- rank -->", rank.ToString()).Replace("<!-- name -->", name).Replace("<!-- code -->", code.ToString()).Replace("<!-- email -->", email);
			return userData;
		}
	}
}