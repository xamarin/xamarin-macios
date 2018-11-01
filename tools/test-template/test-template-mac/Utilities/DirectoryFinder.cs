using System;
using System.IO;
using System.Reflection;

namespace Xamarin.Tests.Templating
{
	static class DirectoryFinder
	{
		public static string FindRootDirectory ()
		{
			var current = Assembly.GetExecutingAssembly ().Location;
			while (!Directory.Exists (Path.Combine (current, "_mac-build")) && current.Length > 1)
				current = Path.GetDirectoryName (current);
			if (current.Length <= 1)
				throw new DirectoryNotFoundException (string.Format ("Could not find the root directory starting from {0}", Environment.CurrentDirectory));
			return Path.GetFullPath (Path.Combine (current, "_mac-build"));
		}

		public static string FindTestDirectory => Path.Combine (FindRootDirectory (), "..", "tests") + "/";

		public static string FindSourceDirectory ()
		{
			string codeBase = Assembly.GetExecutingAssembly ().CodeBase;
			UriBuilder uri = new UriBuilder (codeBase);
			string path = Uri.UnescapeDataString (uri.Path);
			string assemblyDirectory = Path.GetDirectoryName (path);
			return Path.Combine (assemblyDirectory, FindTestDirectory + "common/mac");
		}
	}
}
