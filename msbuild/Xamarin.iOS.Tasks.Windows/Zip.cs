using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Xamarin.iOS.Tasks.Windows {
	internal static class Zip {
		internal static void Extract (string sourceFileName, string destinationPath)
		{
			// We use a temp dir because the extraction dir should not exist for the ZipFile API to work
			var tempExtractionPath = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ().Substring (0, 4));

			ExtractWithSymlinksToDirectory (sourceFileName, tempExtractionPath);

			CopyDirectory (tempExtractionPath, destinationPath);

			// Fixes last write time of all files in the Zip, because the files keep the last write time from the Mac.
			foreach (var filePath in Directory.EnumerateFiles (destinationPath, "*.*", SearchOption.AllDirectories)) {
				new FileInfo (filePath).LastWriteTime = DateTime.Now;
			}

			Directory.Delete (tempExtractionPath, recursive: true);
		}

		private static void CopyDirectory (string source, string destination)
		{
			var sourceDirectoryInfo = new DirectoryInfo (source);

			Directory.CreateDirectory (destination);

			foreach (var file in sourceDirectoryInfo.GetFiles ()) {
				file.CopyTo (Path.Combine (destination, file.Name));
			}

			foreach (var dir in sourceDirectoryInfo.GetDirectories ()) {
				CopyDirectory (dir.FullName, Path.Combine (destination, dir.Name));
			}
		}

		private static void ExtractWithSymlinksToDirectory (string sourceFileName, string destinationDirectoryName)
		{
			// Do the normal extraction first
			using var source = ZipFile.OpenRead (sourceFileName);
			source.ExtractToDirectory (destinationDirectoryName);

			// Read all the symbolic links contained in the zip
			var links = ReadSymbolicLinks (source, destinationDirectoryName).ToList ();
			if (links.Count == 0) {
				// No links, so exit
				return;
			}
			
			// Defensive test, in case the ZipArchive class gets built-in support for symlinks in the future
			if (new FileInfo(links[0].Key).LinkTarget != null) {
				// A link was already created on disk as a symbolic link, so exit
				return;
			}

			// Pass 1 - Delete the fake link files
			foreach (var link in links) {
				File.Delete (link.Key);
			}

			// Pass 2 - Create directory symbolic links
			foreach (var link in links) {
				// Initially, we will assume all links are directory links because there's no way to tell otherwise.
				Directory.CreateSymbolicLink (link.Key, link.Value);
			}

			// Pass 3 - Create file symbolic links
			foreach (var link in links) {
				// If the target directory doesn't exist, then we'll delete the link and recreate it as a file link.
				if (!Directory.Exists (Path.Combine (link.Key, "..", link.Value))) {
					Directory.Delete (link.Key);
					File.CreateSymbolicLink (link.Key, link.Value);
				}
			}
		}

		private static IEnumerable<KeyValuePair<string, string>> ReadSymbolicLinks (ZipArchive archive, string baseDirectory = ".")
		{
			foreach (var entry in archive.Entries.Where (IsSymbolicLink)) {
				var path = Path.Combine (baseDirectory, entry.FullName.Replace ('/', Path.DirectorySeparatorChar));

				using var stream = entry.Open ();
				using var reader = new StreamReader (stream);
				var link = reader.ReadToEnd ().Replace ('/', Path.DirectorySeparatorChar);

				yield return new KeyValuePair<string, string> (path, link);
			}
		}

		// References:
		// https://www.gnu.org/software/libc/manual/html_node/Testing-File-Type.html
		// https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/Interop/Unix/System.Native/Interop.Stat.cs
		private const int S_IFMT = 0xF000;
		private const int S_IFLNK = 0xA000;

		private static bool IsSymbolicLink (ZipArchiveEntry entry) => ((entry.ExternalAttributes >> 16) & S_IFMT) == S_IFLNK;
	}
}
