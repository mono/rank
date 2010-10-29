//
// HtmlOut.cs: Grabs data from the template and formats the user output.
//
// Author:
//   David Mulder (dmulder@novell.com)
//
// Copyright (C) 2010 Novell, Inc (www.novell.com)
// 

using System;
using System.IO;
using System.Reflection;

namespace getRank
{
	public class HtmlOut
	{
		private static string DIR = getDir();
		internal static string header = "";
		internal static string userData = "";
		internal static string footer = "";
		internal static string userHeader = "";
		internal static string projData = "";
		internal static string userFooter = "";
		
		/// <summary>
		/// Gets the directory where the template is located.
		/// </summary>
		/// <returns>
		/// Template directory <see cref="System.String"/>
		/// </returns>
		private static string getDir()
		{
			return Directory.GetCurrentDirectory() + "/htdocs/";
		}
		
		/// <summary>
		/// Reads the template and sections of the template to header, footer, and user strings.
		/// </summary>
		internal static void Template()
		{
			StreamReader read = File.OpenText(DIR + "template.html");
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
			userData = template.Substring(front_user + 19, end_user - aft_front_user);
			footer = template.Substring(aft_end_user, template.Length - aft_end_user);
			UserTemplate(userData);
		}
		
		/// <summary>
		/// Creates an html instance of a user.
		/// </summary>
		/// <param name="name">
		/// User's name <see cref="System.String"/>
		/// </param>
		/// <param name="email">
		/// User's e-mail address <see cref="System.String"/>
		/// </param>
		/// <param name="rank">
		/// User's rank <see cref="System.Int32"/>
		/// </param>
		/// <param name="code">
		/// User's contributed lines of code <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// The html string representing the user <see cref="System.String"/>
		/// </returns>
		internal static string UserRank(User user, int rank)//string name, string email, int rank, int code)
		{
			string data = userHeader.Replace("<!-- rank -->", rank.ToString()).Replace("<!-- name -->", user.name).Replace("<!-- code -->", user.CodeAdded().ToString()).Replace("<!-- email -->", user.email[0]);
			foreach (Project proj in user.projects)
			{
				data += projData.Replace("<!-- Project -->", proj.name).Replace("<!-- projCode -->", "+" + proj.CodeAdded().ToString() + " -" + proj.CodeRemoved().ToString());
			}
			data += userFooter.Replace("<!-- rank -->", rank.ToString()).Replace("<!-- name -->", user.name).Replace("<!-- code -->", user.CodeAdded().ToString()).Replace("<!-- email -->", user.email[0]);
			data += "<p>Updated " + DateTime.Now.ToString() + "</p>";
			return data;
		}
		
		private static void UserTemplate(string template)
		{
			int front_user = template.IndexOf("<!-- Data -->");
			int aft_front_user = front_user + 13;
			int end_user = template.IndexOf("<!-- End Data -->");
			int aft_end_user = end_user + 17;
			userHeader = template.Substring(0, front_user);
			projData = template.Substring(front_user + 13, end_user - aft_front_user);
			userFooter = template.Substring(aft_end_user, template.Length - aft_end_user);
		}
	}
}