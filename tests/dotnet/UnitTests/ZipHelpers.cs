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

		public static List<string> ListInnerZip (string file, string innerZipFileName)
		{
			using var zip = ZipFile.OpenRead (file);
			var innerZipEntry = zip.GetEntry (innerZipFileName);
			if (innerZipEntry is null)
				return new List<string> ();

			using Stream innerZipStream = innerZipEntry.Open ();
			using ZipArchive innerZip = new ZipArchive (innerZipStream, ZipArchiveMode.Read);
			return innerZip.Entries.Select (entry => entry.FullName.TrimEnd ('/').Replace ('/', Path.DirectorySeparatorChar)).ToList ();
		}

		public static string? GetString (string zipfile, string filename)
		{
			using var zip = ZipFile.OpenRead (zipfile);
			var entry = zip.GetEntry (filename);
			if (entry is null)
				return null;

			using var entryStream = entry.Open ();
			using var reader = new StreamReader (entryStream);
			return reader.ReadToEnd ();
		}

		public static string? GetInnerString (string zipfile, string innerZipFileName, string filename)
		{
			using var zip = ZipFile.OpenRead (zipfile);
			var innerZipEntry = zip.GetEntry (innerZipFileName);
			if (innerZipEntry is null)
				return null;

			using Stream innerZipStream = innerZipEntry.Open ();
			using ZipArchive innerZip = new ZipArchive (innerZipStream, ZipArchiveMode.Read);
			var innerEntry = innerZip.GetEntry (filename);
			if (innerEntry is null)
				return null;

			using var innerStream = innerEntry.Open ();
			using var reader = new StreamReader (innerStream);
			return reader.ReadToEnd ();
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
