//
// Database.cs: Retrieves data from the database. When the application has finished retrieving data from other sources,
// the database is cleared and everything is returned to be stored in the database.
//
// Author:
//   David Mulder (dmulder@novell.com)
//
// Copyright (C) 2011 Novell (http://www.novell.com)
// 

using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;

namespace Rankdblib
{
	public class Database
	{	
		private SqliteConnection db = getDb();
		
		static private SqliteConnection getDb()
		{
			string homePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? Environment.GetEnvironmentVariable("HOME") : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
			string connection = "DbLinqProvider=Sqlite; Data Source=\"" + homePath + "/MonoRankDatabase.sqlite\"";
			SqliteConnection conn = new SqliteConnection (connection);
	        return conn;
		}
		
		public Database()
		{
			db.Open();
		}
		
		~Database()
		{
			db.Close();
		}
		
		public List<Users> RetrieveUsers()
		{
			List<Users> users = new List<Users>();
			Users user;
			
			SqliteCommand retrieveUsers = db.CreateCommand();
			retrieveUsers.CommandText = "SELECT rowid, UserName, MailingListMessage, ResolvedBugs FROM User";
			SqliteDataReader reader = retrieveUsers.ExecuteReader();
			while(reader.Read())
			{
				user = new Users();
				users.Add(user);
				SqliteCommand retrieveEmails = db.CreateCommand();
				retrieveEmails.CommandText = "SELECT Email, UserID FROM Address WHERE UserID=" + reader["rowid"].ToString();
				SqliteDataReader addressReader = retrieveEmails.ExecuteReader();
				while(addressReader.Read())
				{
					user.email.Add(addressReader["Email"].ToString());
				}
				user.Name = reader["UserName"].ToString();
				user.MailingListMessages = int.Parse(reader["MailingListMessage"].ToString());
				user.BugsClosed = int.Parse(reader["ResolvedBugs"].ToString());
				
				SqliteCommand retrieveProjects = db.CreateCommand();
				retrieveProjects.CommandText = "SELECT rowid, ProjName, CodeCurved, CodeAdded, CodeRemoved, UserID FROM Project WHERE UserID=" + reader["rowid"].ToString();
				SqliteDataReader projectReader = retrieveProjects.ExecuteReader();
				while(projectReader.Read())
				{
					Projects project = new Projects(projectReader["ProjName"].ToString());
					user.projects.Add(project);
					project.CodeCurved = int.Parse(projectReader["CodeCurved"].ToString());
					project.CodeAdded = int.Parse(projectReader["CodeAdded"].ToString());
					project.CodeRemoved = int.Parse(projectReader["CodeRemoved"].ToString());
					
					SqliteCommand retrieveData = db.CreateCommand();
					retrieveData.CommandText = "SELECT * FROM Data WHERE ProjID=" + projectReader["rowid"].ToString();
					SqliteDataReader dataReader = retrieveData.ExecuteReader();
					while(dataReader.Read())
					{
						project.commits.Add(dataReader["CommitID"].ToString());
					}
				}
			}
			
			return users;
		}
		
		public void Clear()
		{
			SqliteCommand clearUser = db.CreateCommand();
			clearUser.CommandText = "DELETE FROM User";
			clearUser.ExecuteNonQuery();
			
			SqliteCommand clearAddress = db.CreateCommand();
			clearAddress.CommandText = "DELETE FROM Address";
			clearAddress.ExecuteNonQuery();
			
			SqliteCommand clearProject = db.CreateCommand();
			clearProject.CommandText = "DELETE FROM Project";
			clearProject.ExecuteNonQuery();
			
			SqliteCommand clearData = db.CreateCommand();
			clearData.CommandText = "DELETE FROM Data";
			clearData.ExecuteNonQuery();
		}
		
		public void AddUser(Users user)
		{
			SqliteCommand addUser = db.CreateCommand();
			addUser.CommandText = "INSERT INTO User VALUES(\""
				+ user.Name + "\", "
				+ user.MailingListMessages + ", "
				+ user.BugsClosed
				+ ")";
			addUser.ExecuteNonQuery();
			
			int id = GetID("User", "UserName=\"" + user.Name + "\"");
			foreach (Projects project in user.projects)
			{
				AddProject(project, id);
			}
			foreach (string email in user.email)
			{
				AddEmail(email, id);
			}
		}
		
		private void AddEmail(string email, int userid)
		{
			SqliteCommand addEmail = db.CreateCommand();
			addEmail.CommandText = "INSERT INTO Address VALUES(\""
				+ email + "\", "
				+ userid
				+ ")";
			addEmail.ExecuteNonQuery();
		}
		
		private void AddProject(Projects project, int userid)
		{
			SqliteCommand addProject = db.CreateCommand();
			addProject.CommandText = "INSERT INTO Project VALUES(\""
				+ project.name + "\", "
				+ project.CodeCurved + ", "
				+ project.CodeAdded + ", "
				+ project.CodeRemoved + ", "
				+ userid
				+ ")";
			addProject.ExecuteNonQuery();
			int id = GetID("Project", "UserID=" + userid);
			AddData(project, id);
		}
		
		private void AddData(Projects project, int projid)
		{
			foreach (string commit in project.commits)
			{
				SqliteCommand addData = db.CreateCommand();
				addData.CommandText = "INSERT INTO Data VALUES(\"" + commit + "\", " + projid + ")";
				addData.ExecuteNonQuery();
			}
		}
		
		private int GetID(string table, string column)
		{
			SqliteCommand getID = db.CreateCommand();
			getID.CommandText = "SELECT rowid FROM " + table + " WHERE " + column;
			SqliteDataReader reader = getID.ExecuteReader();
			string rowid = reader["rowid"].ToString();
			return int.Parse(rowid);
		}
	}
}

