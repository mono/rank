//
// Gravatar.cs: Returns a Gravatar image URL.
//
// Author:
//   David Mulder (dmulder@novell.com)
//
// Copyright (C) 2010 Novell, Inc (www.novell.com)
// 

using System;
using System.Net;
using System.Threading;
using System.Security.Cryptography;

namespace getRank
{
	public class Gravatar
	{
		private string BaseURL = "http://www.gravatar.com/avatar/{0}?default=404&s={1}&r={2}";
		
		private string Email {get; set;}
		private Ratings Rating {get; set;}
		private int Size {get; set;}
		
		public enum Ratings
        {
            g, pg, r, x
        }
		
		public Gravatar (string email, Ratings rating, int size)
		{
			Email = email;
			Rating = rating;
			Size = size;
		}
		
		/// <summary>
		/// Return the URL hash of the user's Gravatar image.
		/// </summary>
		public string GravatarURL()
		{
			string hash = MD5();
			string url = string.Format(BaseURL, hash, Size, Rating);
			Uri urlCheck = new Uri(url);
			WebRequest request = WebRequest.Create(urlCheck);
			request.Timeout = 1000;
			try
			{
				Thread.Sleep(1000);
				WebResponse response = request.GetResponse();
			}
			catch
			{
				return "img/monkey.png";
			}
			return url;
		}
		
		/// <summary>
		/// Hash the email address with MD5 encryption.
		/// </summary>
		private string MD5()
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hash = System.Text.Encoding.ASCII.GetBytes(Email.ToLower().Trim());
			hash = md5.ComputeHash(hash);
			
			string result = "";
			
			foreach (byte b in hash)
			{
				result += b.ToString("x2");
			}
			
			return result;
		}
	}
}

