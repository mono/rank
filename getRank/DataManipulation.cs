//
// DataManipulation.cs: Handles data recovery and manipulation of git and bugzilla data
// in order to create commit and resolution rankings.
// (Doesn't yet handle bugzilla data)
//
// Author:
//   David Mulder (dmulder@novell.com)
//
// Copyright (C) 2010 Novell (http://www.novell.com)
// 
 
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace getRank
{
	public class DataManipulation
	{
		internal List<User> users = new List<User>();
		
		public DataManipulation (string directory, string start_date)
		{
			DateTime start = GetStartDate(start_date);
			GetDirectoryStructure(directory, start);
			GetMailingListData(start);
		}
		
		private DateTime GetStartDate(string start_date)
		{
			DateTime start = DateTime.Now;
			try
			{
				start = DateTime.Parse(start_date);
			}
			catch
			{
				start = DateTime.Now.AddDays(-21);
			}
			return start;
		}
		
		/// <summary>
		/// Retreives contribution data from the mailing lists.
		/// </summary>
		private void GetMailingListData(DateTime start_date)
		{
			string emails = GetMailingListEmails();
			string[] splitValue = new string[1];
			splitValue[0] = "\n";
			string[] emailLines = emails.Split(splitValue, StringSplitOptions.RemoveEmptyEntries);
			DateTime dtDate = new DateTime();
			
			for (int i = 0; i < emailLines.Length; i++)
			{
				try
				{
					if (emailLines[i].Contains("Date: "))
					{
						string date = emailLines[i].Replace("Date: ", "").Substring(5, 11);
						dtDate = DateTime.Parse(date);
					}
					else if (emailLines[i].Contains("From: "))
					{
						if (dtDate > start_date)
						{
							string nameEmailLine = emailLines[i].Replace("From: ", "");
							string name = nameEmailLine.Substring(0, nameEmailLine.IndexOf('<') - 1).Replace("\"", "").Trim();
							string email = nameEmailLine.Trim().Substring(name.Length, nameEmailLine.Length - name.Length).Replace("<", "").Replace(">", "").Trim();
							
							if (name.ToLower().Contains("utf-8"))
							{
								name = email.Substring(0, email.IndexOf('@'));
								name.Replace(".", " ");
							}
							
							name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
							
							if (name.Contains(","))
							{
								string[] names = name.Split(',');
								name = names[1] + " " + names[0];
								name = name.Trim();
							}
							
							User user;
							name = UserName(email, name);
							if (!UserExists(email, name))
							{
								user = new User(email, name);
								users.Add(user);
							}
							else
							{
								user = iUser(email);
							}
							user.MailingListMessages(1);
						}	
					}
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
		
		private string GetMailingListEmails()
		{
			string homePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? Environment.GetEnvironmentVariable("HOME") : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
			StreamReader read;
			read = File.OpenText(homePath + "/monolists");
			string emails = "";
			while (!read.EndOfStream)
			{
				emails += read.ReadLine() + Environment.NewLine;
			}
			
			return emails;
		}
		
		/// <summary>
		/// Grabs git log data from each sub directory in the current directory.
		/// </summary>
		/// <param name="directory">
		/// The current (parent) directory <see cref="System.String"/>
		/// </param>
		private void GetDirectoryStructure(string directory, DateTime start_date)
		{
			string[] folders = Directory.GetDirectories(directory);
			
			foreach (string dir in folders)
			{
				string[] dir_struc = dir.Split('/');
				GetGitData(dir, start_date, dir_struc[dir_struc.Length - 1]);
			}
		}
		
		/// <summary>
		/// Retrieves commit data from git logs.
		/// </summary>
		/// <returns>
		/// A string containing the git log <see cref="System.String"/>
		/// </returns>
		private void GetGitData(string directory, DateTime start, string project)
		{	
			UpdateRepo(directory);
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.WorkingDirectory = directory;
			p.StartInfo.FileName = "git";
			p.StartInfo.Arguments = @"log --no-merges --cherry-pick --shortstat --pretty=format:'%H;%an;%ce' --since=" + start.ToShortDateString();
			p.Start();
			string data = p.StandardOutput.ReadToEnd();
			ParseGitData(data, project);
		}
		
		/// <summary>
		/// Pulls updates from the current repo.
		/// </summary>
		/// <param name="directory">
		/// Directory of current repo <see cref="System.String"/>
		/// </param>
		private void UpdateRepo(string directory)
		{
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.WorkingDirectory = directory;
			p.StartInfo.FileName = "git";
			p.StartInfo.Arguments = "pull";
			p.Start();
		}
		
		/// <summary>
		/// Encapsulates git data in an object.
		/// </summary>
		/// <param name="data">
		/// git data<see cref="System.String"/>
		/// </param>
		private void ParseGitData(string data, string project)
		{
			//user[0] is the commit #, user[1] is the name, and user[2] is email.
			char[] chars = new char[2];
			chars[0] = '\r';
			chars[1] = '\n';
			string[] sdata = data.Split(chars);
			User previousUser = new User("", "");

			foreach (string line in sdata)
			{
				if (line.Contains("files changed"))
				{
					string[] changes = line.Split(',');
					int added = int.Parse(changes[1].Trim().Split(' ')[0]);
					int removed = int.Parse(changes[2].Trim().Split(' ')[0]);
					previousUser.CodeAdded(added, project);
					previousUser.CodeRemoved(removed, project);
					if (added > removed)
					{
						previousUser.CodeCurved(added - removed, project);
					}
					else if (removed > added)
					{
						previousUser.CodeCurved(removed - added, project);
					}
				}
				else if (line.Trim() != "")
				{
					string[] user = line.Split(';');
					if (UserExists(user[2], user[1]))
					{
						iUser(user[2]).AddCommit(user[0], project);
						previousUser = iUser(user[2]);
					}
					else
					{
						User aUser = new User(user[2], user[1]);
						users.Add(aUser);
						aUser.AddCommit(user[0], project);
						previousUser = aUser;
					}
				}
			}
		}
		
		/// <summary>
		/// Gets a user object.
		/// </summary>
		/// <param name="email">
		/// The user's email address<see cref="System.String"/>
		/// </param>
		/// <returns>
		/// The user's object<see cref="User"/>
		/// </returns>
		private User iUser(string email)
		{
			User iuser = null;
			foreach (User user in users)
			{
				if (user.email.Contains(email))
				{
					iuser = user;
				}
			}
			if (iuser == null)
			{
				throw new Exception("User was not found.");
			}
			
			return iuser;
		}
		
		/// <summary>
		/// Checks if a user account has been added to the User list.
		/// </summary>
		/// <param name="email">
		/// A User email address<see cref="System.String"/>
		/// </param>
		/// <returns>
		/// True if the user already exists. False if user has not been created.<see cref="System.Boolean"/>
		/// </returns>
		private bool UserExists(string email, string name)
		{
			bool exists = false;
			foreach (User user in users)
			{
				string new_name = name.Split()[0] + name.Split()[name.Split().Length - 1];
				string existing_name = user.name.Split()[0] + user.name.Split()[user.name.Split().Length - 1];
				if (user.email.Contains(email) && existing_name == new_name)
				{
					exists = true;
				}
				else if(existing_name == new_name)
				{
					exists = true;
					user.email.Add(email);
				}
			}
			return exists;
		}
		
		private string UserName(string email, string defaultname)
		{
			string name = defaultname;
			foreach (User user in users)
			{
				if (user.email.Contains(email))
				{
					name = user.name;
				}
			}
			return name;
		}
		
		/// <summary>
		/// Sorts users by number of commits.
		/// </summary>
		/// <returns>
		/// Sorted array of Users <see cref="User[]"/>
		/// </returns>
		internal User[] UserRanks()
		{
			int STOP = users.Count - 1;
			User[] ranks = new User[users.Count];
			
			for (int i = 0; i <= STOP; i++)
			{
				int score = 0;
				User highRanking = null;
				foreach (User user in users)
				{
					if (user.Score() >= score)
					{
						score = user.Score();
						highRanking = user;
					}
				}
				ranks[i] = highRanking;
				users.Remove(highRanking);
			}
			return ranks;
		}
	}
	
	internal class User
	{
		internal List<string> email = new List<string>();
		internal string name {get; set;}
		internal List<Project> projects = new List<Project>();
		internal int mailingListMessages = 0;
		
		/// <summary>
		/// Sets the user information.
		/// </summary>
		/// <param name="inEmail">
		/// User's e-mail address <see cref="System.String"/>
		/// </param>
		/// <param name="inName">
		/// User's name <see cref="System.String"/>
		/// </param>
		internal User(string inEmail, string inName)
		{
			email.Add(inEmail);
			name = inName;
		}
		
		/// <summary>
		/// Returns a cumulative score based on contribution to Mono.
		/// </summary>
		internal int Score()
		{
			int score = CodeCurved();
			
			if (score < 0)
			{
				score = score * -1;
			}
			
			score = ((score * CommitCount()) + (MailingListMessages() * 5));
			return score;
		}
		
		/// <summary>
		/// Returns the number of messages a user has contributed
		/// in the mailing lists.
		/// </summary>
		internal int MailingListMessages()
		{
			return mailingListMessages;
		}
		
		/// <summary>
		/// Adds to the number of messages that the user has sent to
		/// the mailing lists.
		/// </summary>
		internal void MailingListMessages(int count)
		{
			mailingListMessages += count;
		}
		
		/// <summary>
		/// Total Number of commits sent to Mono.
		/// </summary>
		internal int CommitCount()
		{
			int count = 0;
			foreach (Project proj in projects)
			{
				count += proj.CommitCount();
			}
			return count;
		}
		
		/// <summary>
		/// Adds a commit to the array if the data hasn't already been submitted.
		/// </summary>
		/// <param name="commit">
		/// Commit ID <see cref="System.String"/>
		/// </param>
		internal void AddCommit(string commit, string project)
		{
			GetProject(project).AddCommit(commit);
		}
		
		internal void CodeCurved(int value, string project)
		{
			GetProject(project).CodeCurved = value;
		}
		
		internal int CodeCurved()
		{
			int code = 0;
			foreach (Project proj in projects)
			{
				code += proj.CodeCurved;
			}
			return code;
		}
		
		/// <summary>
		/// Add lines of contributed code.
		/// </summary>
		/// <param name="value">
		/// Lines of code to add <see cref="System.Int32"/>
		/// </param>
		internal void CodeAdded(int value, string project)
		{
			GetProject(project).CodeAdded = value;
		}
		
		/// <summary>
		/// Gets the amount of code added.
		/// </summary>
		internal int CodeAdded()
		{
			int code = 0;
			foreach (Project proj in projects)
			{
				code += proj.CodeAdded;
			}
			return code;
		}
		
		/// <summary>
		/// Add lines of removed code.
		/// </summary>
		internal void CodeRemoved(int value, string project)
		{
			GetProject(project).CodeRemoved = value;
		}
		
		/// <summary>
		/// Gets the amount of code removed.
		/// </summary>
		internal int CodeRemoved()
		{
			int code = 0;
			foreach (Project proj in projects)
			{
				code += proj.CodeRemoved;
			}
			return code;
		}
		
		/// <summary>
		/// Find the Project, else create the project.
		/// </summary>
		internal Project GetProject(string project)
		{
			foreach (Project proj in projects)
			{
				if (proj.name == project)
				{
					return proj;
				}
			}
			Project temp = new Project(project);
			projects.Add(temp);
			return temp;
		}
	}
	
	internal class Project
	{
		internal string name;
		private int codeCurved = 0;
		private int codeAdded = 0;
		private int codeRemoved = 0;
		private List<string> commits = new List<string>();
		
		internal int RedPercent()
		{
			return (codeRemoved * 100) / (codeRemoved + codeAdded);
		}
		
		/// <summary>
		/// Represents a code curve based on individual commits.
		/// </summary>
		internal int CodeCurved
		{
			get
			{
				return codeCurved;
			}
			set
			{
				codeCurved += value;
			}
		}
		
		/// <summary>
		/// Represents the amount of code contributed to Mono.
		/// </summary>
		internal int CodeAdded
		{
			get
			{
				return codeAdded;
			}
			set
			{
				codeAdded += value;
			}
		}
		
		/// <summary>
		/// Represents the amount of code removed from Mono.
		/// </summary>
		internal int CodeRemoved
		{
			get
			{
				return codeRemoved;	
			}
			set
			{
				codeRemoved += value;
			}
		}
		
		/// <summary>
		/// Constructor sets the name of the project.
		/// </summary>
		/// <param name="project">
		/// The Project name <see cref="System.String"/>
		/// </param>
		internal Project(string project)
		{
			name = project;
		}
		
		/// <summary>
		/// Adds a commit to the array if the data hasn't already been submitted.
		/// </summary>
		/// <param name="commit">
		/// Commit ID <see cref="System.String"/>
		/// </param>
		internal void AddCommit(string commit)
		{
			commits.Add(commit);
		}
		
		/// <summary>
		/// Number of commits.
		/// </summary>
		internal int CommitCount()
		{
			return commits.Count;
		}
	}
}