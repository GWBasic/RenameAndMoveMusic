using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RenameAndMoveMusic
{
	public class Placement
	{
		public Placement (SongInformation songInformation)
		{
			this.songInformation = songInformation;

			if (songInformation.IsCompilation)
			{
				if (null == songInformation.Album)
				{
					this.parentDirs = new string[] { "Compilations" };
				}
				else
				{
					this.parentDirs = new string[] { "Compilations", songInformation.Album };
				}
			}
			else
			{
				if (null == songInformation.Artist)
				{
					if (null == songInformation.Album)
					{
						this.parentDirs = new string[0];
					}
					else
					{
						this.parentDirs = new string[] { songInformation.Album };
					}
				}
				else
				{
					if (null == songInformation.Album)
					{
						this.parentDirs = new string[] { songInformation.Artist };
					}
					else
					{
						this.parentDirs = new string[] { songInformation.Artist, songInformation.Album };
					}
				}
			}

			if (null == songInformation.Title)
			{
				this.filename = Path.GetFileName(songInformation.Path);
			}
			else if (null == songInformation.Track)
			{
				this.filename = songInformation.Title + Path.GetExtension(songInformation.Path);
			}
			else
			{
				this.filename = string.Format(
					"{0} - {1}{2}",
					songInformation.Track.Value.ToString("D2"),
					songInformation.Title,
					Path.GetExtension(songInformation.Path));
			}
		}

		public IEnumerable<string> ParentDirs 
		{
			get 
			{
				foreach (var dir in this.parentDirs)
					yield return this.Sanitize(dir);
			}
		}
		private readonly IEnumerable<string> parentDirs;

		public string Filename 
		{
			get { return this.Sanitize(this.filename); }
		}
		private readonly string filename;

		public SongInformation SongInformation 
		{
			get { return songInformation; }
		}
		private readonly SongInformation songInformation;

		/// <summary>
		/// All of the invalid charachters
		/// </summary>
		private static readonly HashSet<char> invalidChars = new HashSet<char>(
			Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Concat(new char[] {'#', '/', '\\', ':', '?', '"', '\'', '“', '”', '‘', '’', '&'}));

		/// <summary>
		/// Returns a string that converts invalid file system characters to valid characters
		/// </summary>
		private string Sanitize(string toSanitize)
		{
			toSanitize = toSanitize.Normalize();

			var sanitized = new StringBuilder(toSanitize.Length);

			foreach (var c in toSanitize)
				if (!invalidChars.Contains(c))
					sanitized.Append(c);

			return sanitized.ToString();
		}
	}
}

