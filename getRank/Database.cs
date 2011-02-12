using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;

namespace getRank
{
	public class Database
	{	
		private SqliteConnection db = getDb();
		
		static private SqliteConnection getDb()
		{
			SqliteConnection conn = new SqliteConnection (
                "DbLinqProvider=Sqlite;" + 
                "Data Source=MonoRankDatabase.sqlite"
	        );
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
		
		internal void AddUser(Users user)
		{
			if (!user.Present)
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
			else
			{
				//Replace the user
			}
		}
		
		internal void AddEmail(string email, int userid)
		{
			SqliteCommand addEmail = db.CreateCommand();
			addEmail.CommandText = "INSERT INTO Address VALUES(\""
				+ email + "\", "
				+ userid
				+ ")";
			addEmail.ExecuteNonQuery();
		}
		
		internal void AddProject(Projects project, int userid)
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
		}
		
		internal int GetID(string table, string column)
		{
			SqliteCommand getID = db.CreateCommand();
			getID.CommandText = "SELECT rowid FROM " + table + " WHERE " + column;
			SqliteDataReader reader = getID.ExecuteReader();
			string rowid = reader["rowid"].ToString();
			return int.Parse(rowid);
		}
	}
}

