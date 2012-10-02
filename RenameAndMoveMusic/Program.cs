using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameAndMoveMusic
{
	class Program
	{
		public static void Main (string[] args)
		{
			if (2 != args.Length)
			{
				Console.WriteLine("RenameAndMoveMusic [source directory] [destination directory]");
				return;
			}

			var sourceDir = args[0].Normalize();
			var destinationDir = args[1].Normalize();

			try
			{
				foreach (var path in Program.GetAllMP3AndM4AFiles(sourceDir))
				{
					try
					{
						var songInformation = new SongInformation(path);

						/*Console.WriteLine(
@"{0}
	Album: {1}
	Artist: {2}
	Title: {3}
	Track: {4}
	IsCompilation: {5}",
							path,
							songInformation.Album,
							songInformation.Artist,
							songInformation.Title,
							songInformation.Track,
							songInformation.IsCompilation);*/

						var placement = new Placement(songInformation);

						/*foreach (var dir in placement.ParentDirs)
							Console.WriteLine("\t\t" + dir);

						Console.WriteLine("\t\t" + placement.Filename);*/

						Program.Move(destinationDir, path, placement);
					}
					catch (Exception e)
					{
						Console.WriteLine("Problem with " + path);
						var ex = e;
						
						do
						{
							Console.WriteLine(ex);
							ex = ex.InnerException;
						} while (null != ex);
					}
				}

				Program.Clean(destinationDir);
			}
			catch (Exception e)
			{
				var ex = e;

				do
				{
					Console.WriteLine(ex);
					ex = ex.InnerException;
				} while (null != ex);
			}
		}

		private static IEnumerable<string> GetAllMP3AndM4AFiles(string sourceDir)
		{
			foreach (var filePath in Program.GetAllFiles(sourceDir))
			{
				if (filePath.EndsWith(".mp3"))
					yield return filePath;
				else if (filePath.EndsWith(".m4a"))
					yield return filePath;
			}
		}

		private static IEnumerable<string> GetAllFiles(string sourceDir)
		{
			foreach (var subDir in Directory.GetDirectories(sourceDir))
				foreach (var filePath in Program.GetAllFiles(subDir))
					yield return filePath;

			foreach (var filePath in Directory.GetFiles(sourceDir))
				yield return filePath.Normalize();
		}

		/// <summary>
		/// All of the paths that are verified to exist
		/// </summary>
		private static readonly HashSet<string> verified = new HashSet<string>();

		/// <summary>
		/// Verifies that the path exists, creating it if it doesn't
		/// </summary>
		private static void VerifyPath(string destinationPath)
		{
			if (Program.verified.Contains(destinationPath))
				return;

			if (!Directory.Exists(destinationPath))
				Directory.CreateDirectory(destinationPath);

			Program.verified.Add(destinationPath);
		}

		/// <summary>
		/// Moves the file based on instructions in the placement
		/// </summary>
		private static void Move(string destinationDir, string path, Placement placement)
		{
			var destinationPath = destinationDir;

			foreach (var dir in placement.ParentDirs)
			{
				destinationPath = Path.Combine(destinationPath, dir).Normalize();
				Program.VerifyPath(destinationPath);
			}

			destinationPath = Path.Combine(destinationPath, placement.Filename).Normalize();

			if (path.ToLowerInvariant() != destinationPath.ToLowerInvariant())
			{
				Console.WriteLine(
					"Moving\n\tFrom: {0}\n\tTo:   {1}",
					path,
					destinationPath);

				File.Move(
					sourceFileName: path,
					destFileName: destinationPath);
			}
		}

		/// <summary>
		/// Deletes the path if it contains no files, or if it only contains empty folders
		/// </summary>
		private static void Clean(string path)
		{
			foreach (var subdirectory in Directory.GetDirectories(path))
				Program.Clean(subdirectory);

			if (0 == Directory.GetDirectories(path).Length + Directory.GetFiles(path).Length)
			{
				Console.WriteLine("Deleting empty folder: {0}", path);
				Directory.Delete(path);
			}
		}
	}
}
