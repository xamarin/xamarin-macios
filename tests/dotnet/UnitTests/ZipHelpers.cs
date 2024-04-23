#define TRACE

using System.IO;
using System.IO.Compression;

#nullable enable

namespace Xamarin.Tests {
	public static class ZipHelpers {
		public static List<string> List (string file)
		{
			using var zip = ZipFile.OpenRead (file);
			DumpZipFile (zip, file);
			return zip.Entries.Select (entry => entry.FullName.TrimEnd ('/').Replace ('/', Path.DirectorySeparatorChar)).ToList ();
		}

		public static void DumpZipFile (ZipArchive zip, string path)
		{
#if TRACE
			var entries = zip.Entries;
			Console.WriteLine ($"Viewing zip archive {path} with {entries.Count} entries:");
			foreach (var entry in entries) {
				Console.WriteLine ($"    FullName: {entry.FullName} Name: {entry.Name} Length: {entry.Length} CompressedLength: {entry.CompressedLength} ExternalAttributes: 0x{entry.ExternalAttributes:X}");
			}
#endif
		}
	}
}
