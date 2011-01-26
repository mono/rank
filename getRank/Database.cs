using System;
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
		
		static void AddUser(Users user)
		{
			User aUser = new User();
			aUser.Name = user.name;
			db.User.InsertOnSubmit(aUser);
			foreach (Projects proj in user.projects)
			{
				AddProject(proj, aUser.ProjID);
			}
		}
		
		static void AddProject(Projects project, int id)
		{
			Project aProj = new Project();
			aProj.ProjName = project.name;
			aProj.AddressID = id;
			db.Project.InsertOnSubmit(aProj);
		}
		
	}
}

