//
// HtmlOut.cs: Grabs data from the template and formats the user output.
//
// Author:
//   David Mulder (dmulder@novell.com)
//
// Copyright (C) 2011 Novell, Inc (www.novell.com)
// 

using System;
using System.IO;
using System.Reflection;
using Rankdblib;
using Gravatarlib;

namespace getRank
{
	public class HtmlOut
	{
		private static string directory = "";
		internal static string header = "";
		private static string userData = "";
		internal static string footer = "";
		private static string userHeader = "";
		private static string projData = "";
		private static string userFooter = "";

		internal static string TemplateDirectory
		{
			get
			{
				return directory;
			}
			set
			{
				if (value == "")
				{
					directory = Directory.GetCurrentDirectory() + "/htdocs/";
				}
				else
				{
					directory = value;
				}
			}
		}
		
		/// <summary>
		/// Reads the template and sections of the template to header, footer, and user strings.
		/// </summary>
		internal static void Template()
		{
			StreamReader read = File.OpenText(directory + "/template.html");
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
		internal static string UserRank(Users user, int rank)
		{
			string data = ReplaceKeywords(userHeader, user, rank);
			foreach (Projects proj in user.projects)
			{
				data += ReplaceKeywords(projData, user, rank)
					.Replace("<!-- Project -->", proj.name)
					.Replace("<!-- projCode -->", "+" + proj.CodeAdded.ToString() + " -" + proj.CodeRemoved.ToString())
					.Replace("<!-- BluePercent -->", proj.BluePercent().ToString());
			}
			
			for (int i = 0; i < (4 - user.projects.Count); i++)
			{
				data += "<td></td>";
			}

			data += ReplaceKeywords(userFooter, user, rank);
			return data;
		}
		
		private static string ReplaceKeywords(string html, Users user, int rank)
		{
			Gravatar img = new Gravatar(user.email[0], Gravatar.IconSets.identicon, Gravatar.Ratings.g, 50);
			string gravatar = img.GravatarURL();
			return html.Replace("<!-- rank -->", rank.ToString())
				.Replace("<!-- name -->", user.Name)
				.Replace("<!-- code -->", user.CodeAdded().ToString())
				.Replace("<!-- email -->", user.email[0])
				.Replace("<!-- Gravatar -->", gravatar)
				.Replace("<!-- Image -->", ReplaceImageName(user))
				.Replace("<!-- score -->", user.Score().ToString())
				.Replace("<!-- commits -->", user.CommitCount().ToString());
		}
		
		private static string ReplaceImageName(Users user)
		{
			int GROVE = 50000;
			int TREE = 5000;
			int CRATE = 500;
			int BUNCHES = 50;
			int BUNCH = 0;
			int PEEL = 0;
			
			if (user.Score() > GROVE)
			{
				return "grove.png";
			}
			else if (user.Score() > TREE)
			{
				return "banana_tree.png";
			}
			else if (user.Score() > CRATE)
			{
				return "crate.png";
			}
			else if (user.Score() > BUNCHES)
			{
				return "banana_bunches.png";
			}
			else if (user.Score() > BUNCH)
			{
				return "3_bananas.png";
			}
			else if (user.Score() == PEEL)
			{
				return "peel.png";
			}

			return "";
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