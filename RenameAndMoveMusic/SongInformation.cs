using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameAndMoveMusic
{
	public class SongInformation
	{
		public SongInformation (string path)
		{
			try
			{
				using (var file = new TagLib.Mpeg4.File(path))
				{
					var tags = file.GetTag(TagLib.TagTypes.Apple) as TagLib.Mpeg4.AppleTag;

					this.path = path;
					this.title = tags.Title;
					this.album = tags.Album;
					this.artist = tags.AlbumArtists.FirstOrDefault() ?? tags.Performers.FirstOrDefault();
					this.track = tags.Track > 0 ? (int?)tags.Track : (int?)null;
					this.isCompilation = tags.IsCompilation;
				}
			}
			catch
			{
				using (var file = TagLib.File.Create(path))
				{
					var tags = file.GetTag(TagLib.TagTypes.Id3v2);

					this.path = path;
					this.title = tags.Title;
					this.album = tags.Album;
					this.artist = tags.AlbumArtists.FirstOrDefault() ?? tags.Performers.FirstOrDefault();
					this.track = tags.Track > 0 ? (int?)tags.Track : (int?)null;
					//this.isCompilation = tags.IsCompilation;
				}
			}
		}

		public string Path 
		{
			get { return path; }
		}
		private readonly string path;

		public string Title 
		{
			get { return title;	}
		}
		private readonly string title;

		public string Album 
		{
			get { return album;	}
		}
		private readonly string album;

		public string Artist 
		{
			get { return artist; }
		}
		private readonly string artist;

		public int? Track 
		{
			get { return track; }
		}
		private readonly int? track;

		public bool IsCompilation 
		{
			get { return isCompilation; }
		}
		private readonly bool isCompilation;
	}
}

