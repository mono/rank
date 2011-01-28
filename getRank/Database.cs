using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Mono.Data.Sqlite;

namespace getRank
{
	static public class Database
	{
		static private Main db = getDb();
		
		static private Main getDb()
		{
			var conn = new SqliteConnection (
                "DbLinqProvider=Sqlite;" + 
                "Data Source=MonoRankDatabase.sqlite"
	        );
	        return new Main (conn);
		}
		
		internal static void AddUser(Users user)
		{
			User aUser = new User();
			aUser.UserName = user.Name;
			db.User.InsertOnSubmit(aUser);
			foreach (Projects proj in user.projects)
			{
				AddProject(proj, aUser);
			}
			foreach (string email in user.email)
			{
				AddEmailAddress(aUser, email);
			}
			db.SubmitChanges();
		}
		
		private static void AddEmailAddress(User aUser, string email)
		{
			Address aAddress = new Address();
			aAddress.AddID = aUser.AddID;
			aAddress.Email = email;
			db.Address.InsertOnSubmit(aAddress);
		}
		
		private static void AddProject(Projects project, User aUser)
		{
			Project aProj = new Project();
			aProj.ProjName = project.name;
			aProj.ProjID = aUser.ProjID;
			db.Project.InsertOnSubmit(aProj);
			AddData(project, aProj);
		}
		
		private static void AddData(Projects project, Project aProj)
		{
			Data aData = new Data();
			aData.DataID = aProj.ProjID;
			aData.CodeAdded = project.CodeAdded;
			aData.CodeRemoved = project.CodeRemoved;
			aData.CodeCurved = project.CodeCurved;
			db.Data.InsertOnSubmit(aData);
		}
		
//		internal static List<Users> RetrieveUser()
//		{
//			
//			
//		}
	}
}

