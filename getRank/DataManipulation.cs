//
// DataManipulation.cs: Handles data recovery and manipulation of git and bugzilla data
// in order to create commit and resolution rankings.
//
// Author:
//   David Mulder (dmulder@novell.com)
//
// Copyright (C) 2011 Novell (http://www.novell.com)
// 
 
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Rankdblib;

namespace getRank
{
	public class DataManipulation
	{
		private string homePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? Environment.GetEnvironmentVariable("HOME") : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
		private List<Users> users;
		private Database data = new Database();
		
		public DataManipulation (string directory, string start_date)
		{
			GetDatabaseData();
			DateTime start = GetStartDate(start_date);
			GetDirectoryStructure(directory, start);
			GetMailingListData(start);
			GetBugzillaData(start);
			RemoveJunk();
			WriteDatabaseData();
		}
		
		/// <summary>
		/// Retrieves data from the database.
		/// </summary>
		private void GetDatabaseData()
		{
			users = data.RetrieveUsers();
		}
		
		private void WriteDatabaseData()
		{
			data.Clear();
			foreach (Users user in users)
			{
				data.AddUser(user);
			}
		}
		
		/// <summary>
		/// Parse out junk from the retrieved data.
		/// </summary>
		private void RemoveJunk()
		{
			Regex emailMatch = new Regex(@"^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$");
			List<Users> badUsers = new List<Users>();
			foreach (Users user in users)
			{
				//Remove e-mails that weren't parsed correctly
				List<string> badEmails = new List<string>();

				foreach (string email in user.email)
				{
					if (!emailMatch.IsMatch(email))
					{
						badEmails.Add(email);
					} else if (user.email.Count > 1 && user.email.Contains("lluis") && !user.Name.ToLower().Contains("lluis"))
					{
						badEmails.Add(email);
					} else if (user.email.Count > 1 && user.email.Contains("vargaz@gmail.com") && !user.Name.ToLower().Contains("varga"))
					{
						badEmails.Add(email);
					} else if (user.email.Count > 1 && user.email.Contains("taktaktaktaktaktaktaktaktaktak@gmail.com") && !user.Name.ToLower().Contains("levi"))
					{
						badEmails.Add(email);
					} else if (user.email.Count > 1 && user.email.Contains("sebastien@ximian.com") && !user.Name.ToLower().Contains("sebastien"))
					{
						badEmails.Add(email);
					} else if (user.email.Count > 1 && user.email.Contains("levi@unity3d.com") && !user.Name.ToLower().Contains("levi"))
					{
						badEmails.Add(email);
					}
					
				}
				foreach (string email in badEmails)
				{
					user.email.Remove(email);
				}
				
				if (user.email.Count == 0)
				{
					badUsers.Add(user);
				}
				else if (user.Name.Contains("="))//Fix names that weren't parsed correctly
				{
					user.Name = user.email[0].Split('@')[0];
				}
				else if (user.Name.Contains(">"))
				{
					user.Name = user.Name.Replace(">", "").Trim();
				}
			}
			foreach (Users user in badUsers)
			{
				users.Remove(user);
			}
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
			string listLocation = homePath + "/mail/.inboxes/monolists";
			StreamReader read = File.OpenText(listLocation);

			DateTime dtDate = new DateTime();
			
			while (!read.EndOfStream)
			{
				string line = read.ReadLine();
				if (line.Contains("Date: "))
				{
					try
					{
						string date = line.Replace("Date: ", "").Substring(5, 11);
						dtDate = DateTime.Parse(date);
					}
					catch(Exception e){Console.WriteLine(e.Message);}
				}
				else if (line.Contains("From: "))
				{
					try
					{
						if (dtDate > start_date)
						{
							string nameEmailLine = line.Replace("From: ", "");
							string name = nameEmailLine.Substring(0, nameEmailLine.IndexOf('<') - 1).Replace("\"", "").Trim();
							string email = nameEmailLine.Trim().Substring(name.Length, nameEmailLine.Length - name.Length).Replace("<", "").Replace(">", "").Trim();
							
							if (name.ToLower().Contains("utf-8"))
							{
								name = email.Substring(0, email.IndexOf('@'));
							}
							
							name.Replace(".", " ");
							
							if (name.Contains(","))
							{
								string[] names = name.Split(',');
								name = names[1] + " " + names[0];
								name = name.Trim();
							}

							Users user;
							name = UserName(email, name);
							if (!UserExists(email, name))
							{
								user = new Users(email, name);
								users.Add(user);
							}
							else
							{
								user = GetUser(email);
							}
							user.MailingListMessages++;
						}
					}
					catch(Exception e){Console.WriteLine(e.Message);}
				}
			}
			read.Close();
			StreamWriter empty = new StreamWriter(listLocation);
			empty.Write("");
			empty.Close();
		}
		
		/// <summary>
		/// Parses the messages from bugzilla and inserts the data into the user list.
		/// </summary>
		/// <param name="start_date">
		/// The date to begin using data<see cref="DateTime"/>
		/// </param>
		private void GetBugzillaData(DateTime start_date)
		{
			string listLocation = homePath + "/mail/.inboxes/bugzilla";
			StreamReader read = File.OpenText(listLocation);
			
			List<Users> users = new List<Users>();
			Users user = new Users();
			users.Add(user);
			while (!read.EndOfStream)
			{
				string line = read.ReadLine();
				if (line.Contains("_______________________________________________"))
				{
					user = new Users();
					users.Add(user);
				}
				else if (line.Contains("From ") && line.Contains("@lists.ximian.com"))
				{
					string date = line.Substring(line.IndexOf(' ', 5)).Trim();
					string[] splitDate = date.Split(' ');
					date = splitDate[1] + " " + splitDate[2] + " " + splitDate[4];
					try
					{
						if (DateTime.Parse(date) < start_date)
						{
							users.Remove(user);
						}
					}
					catch(Exception e){Console.WriteLine(e.Message);}
				}
				else if (line.Contains("https://bugzilla.novell.com/show_bug.cgi?id="))
				{
					string bugID = line.Replace("https://bugzilla.novell.com/show_bug.cgi?id=", "");
					string[] bugIDParse = bugID.Split('#');
					user.BugsWorked(bugIDParse[0]);
				}
				else if (line.Contains("Status|"))
				{
					string status = line.Split('|')[2];
					if (status == "RESOLVED")
					{
						user.BugsClosed++;
					}
				}
				else if (line.Contains(" changed:"))
				{
					try
					{
						string name = line.Substring(0, line.IndexOf('<'));
						user.Name = name;
						string email = line.Split('<')[1];
						email = email.Substring(0, email.IndexOf('>'));
						user.email.Add(email);
					}
					catch(Exception e){Console.WriteLine(e.Message);}
				}
				else if (line.Contains("--- Comment #"))
				{
					try
					{
						string name = line.Remove(0, line.IndexOf("from") + 5);
						name = name.Substring(0, name.IndexOf('<')).Trim();
						user.Name = name;
						string email = line.Split('<')[1];
						email = email.Substring(0, email.IndexOf('>'));
						user.email.Add(email);
					}
					catch(Exception e){Console.WriteLine(e.Message);}
				}
			}
			read.Close();
			
			StreamWriter empty = new StreamWriter(listLocation);
			empty.Write("");
			empty.Close();
			
			foreach (Users user2 in users)
			{
				if (user2.email.Count != 0)
				{
					if (UserExists(user2.email[0], user2.Name))
					{
						user = GetUser(user2.email[0]);
						user.BugsClosed += user2.BugsClosed;
						foreach (string bug in user2.BugsWorked())
						{
							user.BugsWorked(bug);
						}
					}
				}
			}
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
			p.StartInfo.Arguments = @"log --no-merges --cherry-pick --shortstat --pretty=format:'%H;%an;%ae' --since=" + start.ToShortDateString();
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
			Users previousUser = new Users();
			bool newcommit = false;

			foreach (string line in sdata)
			{
				if (line.Contains("files changed"))
				{
					if (newcommit)
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
				}
				else if (line.Trim() != "")
				{
					string[] user = line.Split(';');
					if (UserExists(user[2], user[1]))
					{
						previousUser = GetUser(user[2]);
						newcommit = previousUser.AddCommit(user[0], project);
					}
					else
					{
						Users aUser = new Users(user[2], user[1]);
						users.Add(aUser);
						newcommit = aUser.AddCommit(user[0], project);
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
		private Users GetUser(string email)
		{
			Users iuser = null;
			foreach (Users user in users)
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
			foreach (Users user in users)
			{
				string new_name = name.Split()[0] + name.Split()[name.Split().Length - 1];
				string existing_name = user.Name.Split()[0] + user.Name.Split()[user.Name.Split().Length - 1];
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
		
		/// <summary>
		/// Returns a user's name if it is found in the list of users, otherwise returns the default.
		/// </summary>
		/// <param name="email">
		/// The users email address<see cref="System.String"/>
		/// </param>
		/// <param name="defaultname">
		/// A default to return if the name is not found<see cref="System.String"/>
		/// </param>
		/// <returns>
		/// The users name<see cref="System.String"/>
		/// </returns>
		private string UserName(string email, string defaultname)
		{
			string name = defaultname;
			foreach (Users user in users)
			{
				if (user.email.Contains(email))
				{
					name = user.Name;
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
		internal Users[] UserRanks()
		{
			int STOP = users.Count - 1;
			Users[] ranks = new Users[users.Count];
			
			for (int i = 0; i <= STOP; i++)
			{
				int score = 0;
				Users highRanking = null;
				foreach (Users user in users)
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
}