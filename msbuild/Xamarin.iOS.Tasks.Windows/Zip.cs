using System;
using System.IO;
using System.IO.Compression;

namespace Xamarin.iOS.Tasks.Windows {
	internal static class Zip {
		internal static void Extract (string sourceFileName, string destinationPath)
		{
			// We use a temp dir because the extraction dir should not exist for the ZipFile API to work
			var tempExtractionPath = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ().Substring (0, 4));

			ZipFile.ExtractToDirectory (sourceFileName, tempExtractionPath);

			CopyDirectory (tempExtractionPath, destinationPath);

			// Fixes last write time of all files in the Zip, because the files keep the last write time from the Mac.
			foreach (var filePath in Directory.EnumerateFiles (destinationPath, "*.*", SearchOption.AllDirectories)) {
				new FileInfo (filePath).LastWriteTime = DateTime.Now;
			}

			Directory.Delete (tempExtractionPath, recursive: true);
		}

		static void CopyDirectory (string source, string destination)
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
	}
}

