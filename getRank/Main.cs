//
// Main.cs: Handles creation of a ranking page.
//
// Author:
//   David Mulder (dmulder@novell.com)
//
// Copyright (C) 2010 Novell, Inc (www.novell.com)
// 

using System;
using System.IO;

namespace getRank
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string d = Directory.GetCurrentDirectory();
			string s = "";
			string f = "index.html";
			string g = Directory.GetCurrentDirectory();
			
			if (args[0] == "--help" || args[0] == "-h")
			{
				Console.WriteLine(Help());
			}
			else
			{
				for (int i = 0; i < args.Length; i += 2)
				{
					switch(args[i].ToLower())
					{
					case "-d":
					case "--dir":
						d = args[i + 1];
						break;
					case "-s":
					case "--start-date":
						s = args[i + 1];
						break;
					case "-f":
					case "--file-name":
						f = args[i + 1];
						break;
					case "-g":
					case "--git-dir":
						g = args[i + 1];
						break;
					}
				}
				
				DataManipulation manipulator = new DataManipulation(g.Replace("\"", "").Replace("'", ""), s.Replace("\"", "").Replace("'", ""));
				
				FileInfo info = new FileInfo(d.Replace("\"", "").Replace("'", "") + "/" + f.Replace("\"", "").Replace("'", ""));
				StreamWriter writer = info.CreateText();
				
				HtmlOut.Template();
				writer.Write(HtmlOut.header);
			
				User[] ranks = manipulator.UserRanks();
				for (int i = 0; i <= ranks.Length - 1; i++)
				{
					if (ranks[i] != null)
					{
						writer.Write(HtmlOut.UserRank(ranks[i], i + 1));//ranks[i].name, ranks[i].email.GetRange(0, 1)[0], i + 1, ranks[i].Code()));
					}
				}
				writer.Write("<p>Updated " + DateTime.Now.ToString() + "</p>");
				writer.Write(HtmlOut.footer);
				writer.Close();
			}
		}
		
		/// <summary>
		/// Displays the help file.
		/// </summary>
		/// <returns>
		/// A string help message <see cref="System.String"/>
		/// </returns>
		public static string Help()
		{
			return @"Usage: getRank [OPTIONS]

	Gathers stats from git logs and bugzilla (not yet implemented) and
	ranks individuals from 1 to 50 (users below 50 are not included).
	Dumps an html file based on the html template found in the htdocs
	subdirectory.

	-d, --dir		Specifies the directory to output the html file.
				If this argument isn't specified, program
				defaults to current directory.

	-s, --start-date	Specifies the date at which to begin using data.
				If this argument is not specified, 3 weeks prior to
				now is assumed.

	-f, --file-name	Specifies the filename for the html output.
				If this argument isn't specified, program
				defaults to 'index.html'.

	-g, --git-dir		Specifies the parent directory of git repos.
				This directory should be the directory
				above all the repos to be counted in the
				stats. The program will only retrieve log
				data for directories directly below this
				top directory. If this argument isn't
				specified, program defaults to current
				directory.

	-h, --help		Shows this help file.

	In an html template, these tags should enclose the html which
	will be iterated for each user:

	<!-- Begin User -->
	<!-- End User -->
	
	These tags will be replaced by their respective values for
	each user:

	<!-- name -->
	<!-- rank -->
	<!-- commits -->

	Here is an example:

	<!-- Begin User -->
	<tr class = 'tablerow'>
		<td> <!-- rank --> </td>
		<td> <!-- name --> </td>
		<td> <!-- commits --> </td>
	</tr>
	<!-- End User -->
				                  ";
		}
	}
}