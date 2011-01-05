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
		private string BaseURL = "http://www.gravatar.com/avatar/{0}?d={1}&s={2}&r={3}";
		
		private string Email {get; set;}
		private Ratings Rating {get; set;}
		private int Size {get; set;}
		private IconSets IconSet {get; set;}
		
		public enum Ratings
        {
            g, pg, r, x
        }
		
		public Gravatar (string email, IconSets iconset, Ratings rating, int size)
		{
			Email = email;
			IconSet = iconset;
			Rating = rating;
			Size = size;
		}
		
        public enum IconSets
        {
            identicon, monsterid, wavatar
        }
		
		/// <summary>
		/// Return the URL hash of the user's Gravatar image.
		/// </summary>
		public string GravatarURL()
		{
			string hash = MD5();
			string url = string.Format(BaseURL, hash, IconSet, Size, Rating);
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

