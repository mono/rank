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

namespace getRank
{
	public class DataManipulation
	{
		internal List<User> users = new List<User>();
		
		public DataManipulation (string directory, string start_date)
		{
			GetDirectoryStructure(directory, start_date);
		}
		
		/// <summary>
		/// Grabs git log data from each sub directory in the current directory.
		/// </summary>
		/// <param name="directory">
		/// The current (parent) directory <see cref="System.String"/>
		/// </param>
		private void GetDirectoryStructure(string directory, string start_date)
		{
			string[] folders = Directory.GetDirectories(directory);
			
			foreach (string dir in folders)
			{
				GetGitData(dir, start_date);
			}
		}
		
		/// <summary>
		/// Retrieves commit data from git logs.
		/// </summary>
		/// <returns>
		/// A string containing the git log <see cref="System.String"/>
		/// </returns>
		private void GetGitData(string directory, string start_date)
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
			
			UpdateRepo(directory);
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.WorkingDirectory = directory;
			p.StartInfo.FileName = "git";
			p.StartInfo.Arguments = @"log --shortstat --pretty=format:'%H;%an;%ce' --since=" + start.ToShortDateString();
			p.Start();
			string data = p.StandardOutput.ReadToEnd();
			ParseGitData(data);
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
		private void ParseGitData(string data)
		{
			//user[0] is the commit #, user[1] is the name, and user[2] is email.
			char[] chars = new char[2];
			chars[0] = '\r';
			chars[1] = '\n';
			string[] sdata = data.Split(chars);
			User previousUser = new User("", "");
			bool prevNew = true;
			foreach (string line in sdata)
			{
				if (line.Contains("files changed") && prevNew)
				{
					int startIns = line.IndexOf(",") + 1;
					string insertions = line.Substring(startIns, line.IndexOf("insertions") - startIns);
					previousUser.addCode(int.Parse(insertions.Trim()));
				}
				else if (line.Trim() != "")
				{
					string[] user = line.Split(';');
					if (UserExists(user[2], user[1]))
					{
						prevNew = iUser(user[2]).addCommit(user[0]);
						previousUser = iUser(user[2]);
					}
					else
					{
						User aUser = new User(user[2], user[1]);
						users.Add(aUser);
						prevNew = aUser.addCommit(user[0]);
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
				if (user.email.Contains(email))
				{
					exists = true;
				}
				else if(user.name == name)
				{
					exists = true;
					user.email.Add(email);
				}
			}
			return exists;
		}
		
		/// <summary>
		/// Sorts users by number of commits.
		/// </summary>
		/// <returns>
		/// Sorted array of Users <see cref="User[]"/>
		/// </returns>
		internal User[] UserRanks()
		{
			int STOP = 0;
			User[] ranks;
			
			if (users.Count < 50)
			{
				ranks = new User[users.Count];
				STOP = users.Count - 1;
			}
			else
			{
				ranks = new User[50];
				STOP = 49;
			}

			for (int i = 0; i <= STOP; i++)
			{
				int code = 0;
				User highRanking = null;
				foreach (User user in users)
				{
					if (user.code > code)
					{
						code = user.code;
						highRanking = user;
					}
				}
				ranks[i] = highRanking;
				users.Remove(highRanking);
				users.Capacity = users.Count;
			}
			return ranks;
		}
	}
	
	internal class User
	{
		internal List<string> email = new List<string>();
		internal string name {get; set;}
		internal int code = 0;
		internal List<string> commits = new List<string>();
		
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
		/// Adds a commit to the array if the data hasn't already been submitted.
		/// </summary>
		/// <param name="commit">
		/// Commit ID <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Does not already exist <see cref="System.Boolean"/>
		/// </returns>
		internal bool addCommit(string commit)
		{
			if (!commits.Contains(commit))
			{
				commits.Add(commit);
				//Does not already exist.
				return true;
			}
			else
			{
				//Does already exist.
				return false;
			}
		}
		
		/// <summary>
		/// Add lines of contributed code.
		/// </summary>
		/// <param name="value">
		/// Lines of code to add <see cref="System.Int32"/>
		/// </param>
		internal void addCode(int value)
		{
			code += value;
		}
	}
}