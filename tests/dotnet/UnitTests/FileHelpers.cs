using System.IO;

#nullable enable

namespace Xamarin.Tests {
	public static class FileHelpers {
		public static void CopyDirectory (string sourceDir, string destinationDir)
		{
			if (!Directory.Exists (sourceDir))
				throw new DirectoryNotFoundException ($"Unable to copy source dir, it was not found: {sourceDir}");

			if (!Directory.Exists (destinationDir))
				Directory.CreateDirectory (destinationDir);

			foreach (var file in Directory.GetFiles (sourceDir)) {
				var destFile = Path.Combine (destinationDir, Path.GetFileName (file));
				File.Copy (file, destFile, true);
			}

			foreach (var subdir in Directory.GetDirectories (sourceDir)) {
				var destSubdir = Path.Combine (destinationDir, Path.GetFileName (subdir));
				CopyDirectory (subdir, destSubdir);
			}
		}

	}
}
