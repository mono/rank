using System;
using System.Collections.Generic;

namespace Rankdblib
{
	public class Users
	{
		public List<string> email = new List<string>();
		public List<Projects> projects = new List<Projects>();
		private List<string> bugsWorked = new List<string>();
		
		public string Name {get; set;}
		public int MailingListMessages {get; set;}
		public int BugsClosed {get; set;}
		
		/// <summary>
		/// Creates a new user that is not present in the database.
		/// </summary>
		public Users(){Name = "";}
		
		/// <summary>
		/// Sets the user information.
		/// </summary>
		/// <param name="inEmail">
		/// User's e-mail address <see cref="System.String"/>
		/// </param>
		/// <param name="inName">
		/// User's name <see cref="System.String"/>
		/// </param>
		public Users(string inEmail, string inName, bool present)
		{
			MailingListMessages = 0;
			BugsClosed = 0;
			email.Add(inEmail);
			Name = inName;
		}
		
		/// <summary>
		/// Returns a cumulative score based on contribution to Mono.
		/// </summary>
		public int Score()
		{
			int score = CodeCurved();
			
			if (score < 0)
			{
				score = score * -1;
			}
			
			score = ((score * CommitCount()) + (MailingListMessages * 5) + (BugsWorkedCount() * 15) + (BugsClosed * 20));
			return score;
		}
		
		/// <summary>
		/// Total Number of commits sent to Mono.
		/// </summary>
		public int CommitCount()
		{
			int count = 0;
			foreach (Projects proj in projects)
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
		public void AddCommit(string commit, string project)
		{
			GetProject(project).AddCommit(commit);
		}
		
		public void CodeCurved(int value, string project)
		{
			GetProject(project).CodeCurved = value;
		}
		
		public int CodeCurved()
		{
			int code = 0;
			foreach (Projects proj in projects)
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
		public void CodeAdded(int value, string project)
		{
			GetProject(project).CodeAdded = value;
		}
		
		/// <summary>
		/// Gets the amount of code added.
		/// </summary>
		public int CodeAdded()
		{
			int code = 0;
			foreach (Projects proj in projects)
			{
				code += proj.CodeAdded;
			}
			return code;
		}
		
		/// <summary>
		/// Add lines of removed code.
		/// </summary>
		public void CodeRemoved(int value, string project)
		{
			GetProject(project).CodeRemoved = value;
		}
		
		/// <summary>
		/// Gets the amount of code removed.
		/// </summary>
		public int CodeRemoved()
		{
			int code = 0;
			foreach (Projects proj in projects)
			{
				code += proj.CodeRemoved;
			}
			return code;
		}
		
		/// <summary>
		/// Find the Project, else create the project.
		/// </summary>
		public Projects GetProject(string project)
		{
			foreach (Projects proj in projects)
			{
				if (proj.name == project)
				{
					return proj;
				}
			}
			Projects temp = new Projects(project);
			projects.Add(temp);
			return temp;
		}
		
		/// <summary>
		/// Add a bug id, only if it hasn't already been added.
		/// </summary>
		/// <param name="bugid">
		/// The bugs id <see cref="System.String"/>
		/// </param>
		public void BugsWorked(string bugid)
		{
			if (!bugsWorked.Contains(bugid))
			{
				bugsWorked.Add(bugid);
			}
		}
		
		/// <summary>
		/// The number of bugs worked on.
		/// </summary>
		/// <returns>
		/// Number of bugs worked <see cref="System.Int32"/>
		/// </returns>
		public int BugsWorkedCount()
		{
			return bugsWorked.Count;
		}
						
		public List<string> BugsWorked()
		{
			return bugsWorked;
		}
	}
	
	public class Projects
	{
		public string name;
		private int codeCurved = 0;
		private int codeAdded = 0;
		private int codeRemoved = 0;
		public List<string> commits = new List<string>();
		
		public int BluePercent()
		{
			return (codeAdded * 100) / (codeRemoved + codeAdded);
		}
		
		/// <summary>
		/// Represents a code curve based on individual commits.
		/// </summary>
		public int CodeCurved
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
		public int CodeAdded
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
		public int CodeRemoved
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
		public Projects(string project)
		{
			name = project;
		}
		
		/// <summary>
		/// Adds a commit to the array if the data hasn't already been submitted.
		/// </summary>
		/// <param name="commit">
		/// Commit ID <see cref="System.String"/>
		/// </param>
		public void AddCommit(string commit)
		{
			commits.Add(commit);
		}
		
		/// <summary>
		/// Number of commits.
		/// </summary>
		public int CommitCount()
		{
			return commits.Count;
		}
	}
}

