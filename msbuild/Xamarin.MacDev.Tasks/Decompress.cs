using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Bundler;
using Xamarin.Localization.MSBuild;
using Xamarin.MacDev.Tasks;

#nullable enable

namespace Xamarin.MacDev {
	public static class CompressionHelper {
		/// <summary>
		/// Is the specified path a compressed file?
		/// </summary>
		/// <param name="path">The path to check</param>
		/// <returns>True if the path represents a compressed file (by checking the extension)</returns>
		public static bool IsCompressed (string path)
		{
			return path.EndsWith (".zip", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Finds a file from either a directory or a zip file.
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="resources">Path to a directory or a zip archive where to look.</param>
		/// <param name="relativeFilePath">The relative path to find, either in a directory or a zip archive.</param>
		/// <returns>If successful, a stream to the read the file. Otherwise null.</returns>
		public static Stream? TryGetPotentiallyCompressedFile (TaskLoggingHelper log, string resources, string relativeFilePath)
		{
			// Check if we have a zipped resources, and if so, extract the manifest from the zip file
			if (IsCompressed (resources)) {
				if (!File.Exists (resources)) {
					log.LogWarning (MSBStrings.W7107 /* The zip file '{0}' does not exist */, resources);
					return null;
				}
				using var zip = ZipFile.OpenRead (resources);
				var contentEntry = zip.GetEntry (relativeFilePath.Replace ('\\', '/')); // directory separator character is '/' on all platforms in zip files.
				if (contentEntry is null) {
					log.LogWarning (MSBStrings.W7106 /* Expected a file named '{1}' in the zip file {0}. */, resources, relativeFilePath);
					return null;
				}

				using var contentStream = contentEntry.Open ();
				var memoryStream = new MemoryStream ();
				contentStream.CopyTo (memoryStream);
				memoryStream.Position = 0;
				return memoryStream;
			}

			if (!Directory.Exists (resources)) {
				log.LogWarning (MSBStrings.W7111 /* The directory '{0}' does not exist. */, resources);
				return null;
			}

			var contentPath = Path.Combine (resources, relativeFilePath);
			if (!File.Exists (contentPath)) {
				log.LogWarning (MSBStrings.W7108 /* The file '{0}' does not exist. */, contentPath);
				return null;
			}

			return File.OpenRead (contentPath);
		}

		/// <summary>
		/// Extracts the specified resource (may be either a file or a directory) from the given zip file.
		/// A stamp file will be created to avoid re-extracting unnecessarily.
		///
		/// Fails if:
		/// * The resource is or contains a symlink and we're executing on Windows.
		/// * The resource isn't found inside the zip file.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="zip">The zip to search in</param>
		/// <param name="resource">The relative path inside the zip to extract (may be a file or a directory).</param>
		/// <param name="decompressionDir">The location on disk to store the extracted results</param>
		/// <param name="decompressedResource">The location on disk to the extracted resource</param>
		/// <returns></returns>
		public static bool TryDecompress (TaskLoggingHelper log, string zip, string resource, string decompressionDir, List<string> createdFiles, [NotNullWhen (true)] out string? decompressedResource)
		{
			decompressedResource = Path.Combine (decompressionDir, resource);

			var stampFile = decompressedResource.TrimEnd ('\\', '/') + ".stamp";

			if (FileCopier.IsUptodate (zip, stampFile, XamarinTask.GetFileCopierReportErrorCallback (log), XamarinTask.GetFileCopierLogCallback (log), check_stamp: false))
				return true;

			// We use 'unzip' to extract on !Windows, and System.IO.Compression to extract on Windows.
			// This is because System.IO.Compression doesn't handle symlinks correctly, so we can only use
			// it on Windows. It's also possible to set the XAMARIN_USE_SYSTEM_IO_COMPRESSION=1 environment
			// variable to force using System.IO.Compression on !Windows, which is particularly useful when
			// testing the System.IO.Compression implementation locally (with the caveat that symlinks won't
			// be extracted).

			bool rv;
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				rv = TryDecompressUsingSystemIOCompression (log, zip, resource, decompressionDir);
			} else if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_USE_SYSTEM_IO_COMPRESSION"))) {
				rv = TryDecompressUsingSystemIOCompression (log, zip, resource, decompressionDir);
			} else {
				rv = TryDecompressUsingUnzip (log, zip, resource, decompressionDir);
			}

			if (rv) {
				Directory.CreateDirectory (Path.GetDirectoryName (stampFile));
				using var touched = new FileStream (stampFile, FileMode.Create, FileAccess.Write);
				createdFiles.Add (stampFile);
			}

			if (File.Exists (decompressedResource)) {
				createdFiles.Add (decompressedResource);
			} else if (Directory.Exists (decompressedResource)) {
				createdFiles.AddRange (Directory.GetFiles (decompressedResource, "*", SearchOption.AllDirectories));
			} else {
				log.LogWarning ("The extracted file or directory '{0}' could not be found." /* The extracted file or directory '{0}' could not be found. */, decompressedResource);
			}

			return rv;
		}

		// The dir separator character in zip files is always "/", even on Windows
		const char zipDirectorySeparator = '/';

		static bool TryDecompressUsingUnzip (TaskLoggingHelper log, string zip, string resource, string decompressionDir)
		{
			Directory.CreateDirectory (decompressionDir);
			var args = new List<string> {
				"-u", "-o",
				"-d", decompressionDir,
				zip,
			};

			if (!string.IsNullOrEmpty (resource)) {
				using var archive = ZipFile.OpenRead (zip);
				resource = resource.Replace ('\\', zipDirectorySeparator);
				var entry = archive.GetEntry (resource);
				if (entry is null) {
					entry = archive.GetEntry (resource + zipDirectorySeparator);
					if (entry is null) {
						log.LogError (MSBStrings.E7112 /* Could not find the file or directory '{0}' in the zip file '{1}'. */, resource, zip);
						return false;
					}
				}

				var zipPattern = entry.FullName;
				if (zipPattern.Length > 0 && zipPattern [zipPattern.Length - 1] == zipDirectorySeparator) {
					zipPattern += "*";
				}

				args.Add (zipPattern);
			}

			var rv = XamarinTask.ExecuteAsync (log, "unzip", args).Result;
			return rv.ExitCode == 0;
		}

		static bool TryDecompressUsingSystemIOCompression (TaskLoggingHelper log, string zip, string resource, string decompressionDir)
		{
			var rv = true;

			// canonicalize input
			resource = resource.TrimEnd ('/', '\\');
			resource = resource.Replace ('\\', zipDirectorySeparator);
			var resourceAsDir = resource + zipDirectorySeparator;

			using var archive = ZipFile.OpenRead (zip);
			foreach (var entry in archive.Entries) {
				var entryPath = entry.FullName;
				if (entryPath.Length == 0)
					continue;

				if (string.IsNullOrEmpty (resource)) {
					// an empty resource means extract everything, so we want this
				} else if (entryPath.StartsWith (resourceAsDir, StringComparison.Ordinal)) {
					// yep, we want this entry
				} else if (entryPath == resource) {
					// we want this one too
				} else {
					log.LogMessage (MessageImportance.Low, "Did not extract {0} because it didn't match the resource {1}", entryPath, resource);
					// but otherwise nope
					continue;
				}

				// Check if the file or directory is a symlink, and show an error if so. Symlinks are only supported
				// on non-Windows platforms.
				var entryAttributes = ((uint) GetExternalAttributes (entry)) >> 16;
				const uint S_IFLNK = 0xa000; // #define S_IFLNK  0120000  /* symbolic link */
				var isSymlink = (entryAttributes & S_IFLNK) == S_IFLNK;
				if (isSymlink) {
					log.LogError (MSBStrings.E7113 /* Can't process the zip file '{0}' on this platform: the file '{1}' is a symlink. */, zip, entryPath);
					rv = false;
					continue;
				}

				var isDir = entryPath [entryPath.Length - 1] == zipDirectorySeparator;
				var targetPath = Path.Combine (decompressionDir, entryPath.Replace (zipDirectorySeparator, Path.DirectorySeparatorChar));
				if (isDir) {
					Directory.CreateDirectory (targetPath);
				} else {
					Directory.CreateDirectory (Path.GetDirectoryName (targetPath));
					using var streamWrite = File.OpenWrite (targetPath);
					using var streamRead = entry.Open ();
					streamRead.CopyTo (streamWrite);
					log.LogMessage (MessageImportance.Low, "Extracted {0} into {1}", entryPath, targetPath);
				}
			}

			return rv;
		}

		/// <summary>
		/// Compresses the specified resource (may be either a file or a directory) into a zip file.
		///
		/// Fails if:
		/// * The resource is or contains a symlink and we're executing on Windows.
		/// * The resource isn't found inside the zip file.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="zip">The zip to create</param>
		/// <param name="resource">The file or directory to compress.</param>
		/// <returns></returns>
		public static bool TryCompress (TaskLoggingHelper log, string zip, string resource, bool overwrite)
		{
			// We use 'zip' to compress on !Windows, and System.IO.Compression to extract on Windows.
			// This is because System.IO.Compression doesn't handle symlinks correctly, so we can only use
			// it on Windows. It's also possible to set the XAMARIN_USE_SYSTEM_IO_COMPRESSION=1 environment
			// variable to force using System.IO.Compression on !Windows, which is particularly useful when
			// testing the System.IO.Compression implementation locally (with the caveat that if the resource
			// to compress has symlinks, it may not work).

			if (overwrite)
				File.Delete (zip);

			var zipdir = Path.GetDirectoryName (zip);
			if (!string.IsNullOrEmpty (zipdir))
				Directory.CreateDirectory (zipdir);

			bool rv;
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				rv = TryCompressUsingSystemIOCompression (log, zip, resource);
			} else if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_USE_SYSTEM_IO_COMPRESSION"))) {
				rv = TryCompressUsingSystemIOCompression (log, zip, resource);
			} else {
				rv = TryCompressUsingUnzip (log, zip, resource);
			}

			return rv;
		}

		// Will add to an existing zip file (not replace)
		static bool TryCompressUsingUnzip (TaskLoggingHelper log, string zip, string resource)
		{
			var zipArguments = new List<string> ();
			zipArguments.Add ("-9");
			zipArguments.Add ("-r");
			zipArguments.Add ("-y");
			zipArguments.Add (zip);

			var fullPath = Path.GetFullPath (resource);
			var workingDirectory = Path.GetDirectoryName (fullPath);
			zipArguments.Add (Path.GetFileName (fullPath));
			var rv = XamarinTask.ExecuteAsync (log, "zip", zipArguments, workingDirectory: workingDirectory).Result;
			log.LogMessage (MessageImportance.Low, "Created {0} from {1}: {2}", zip, resource, rv.ExitCode == 0);
			return rv.ExitCode == 0;
		}

#if NET
		const CompressionLevel SmallestCompressionLevel = CompressionLevel.SmallestSize;
#else
		const CompressionLevel SmallestCompressionLevel = CompressionLevel.Optimal;
#endif

		// Will add to an existing zip file (not replace)
		static bool TryCompressUsingSystemIOCompression (TaskLoggingHelper log, string zip, string resource)
		{
			var rv = true;

			if (File.Exists (zip)) {
				using var archive = ZipFile.Open (zip, ZipArchiveMode.Update);
				var fullResourcePath = Path.GetFullPath (resource);
				var rootDir = Path.GetDirectoryName (fullResourcePath);
				if (Directory.Exists (resource)) {
					var entries = Directory.GetFileSystemEntries (fullResourcePath, "*", SearchOption.AllDirectories);
					var entriesWithZipName = entries.Select (v => new { Path = v, ZipName = v.Substring (rootDir.Length) });
					foreach (var entry in entriesWithZipName) {
						if (Directory.Exists (entry.Path)) {
							if (entries.Where (v => v.StartsWith (entry.Path, StringComparison.Ordinal)).Count () == 1) {
								// this is a directory with no files inside, we need to create an entry with a trailing directory separator.
								archive.CreateEntry (entry.ZipName + zipDirectorySeparator);
							}
						} else {
							WriteFileToZip (log, archive, entry.Path, entry.ZipName);
						}
					}
				} else if (File.Exists (fullResourcePath)) {
					var zipName = fullResourcePath.Substring (rootDir.Length);
					WriteFileToZip (log, archive, fullResourcePath, zipName);
				} else {
					throw new FileNotFoundException (resource);
				}
			} else {
				ZipFile.CreateFromDirectory (resource, zip, SmallestCompressionLevel, true);
			}

			log.LogMessage (MessageImportance.Low, "Created {0} from {1}", zip, resource);

			return rv;
		}

		static void WriteFileToZip (TaskLoggingHelper log, ZipArchive archive, string path, string zipName)
		{
			var zipEntry = archive.CreateEntry (zipName, SmallestCompressionLevel);
			using var fs = File.OpenRead (path);
			using var zipStream = zipEntry.Open ();
			fs.CopyTo (zipStream);
			log.LogMessage (MessageImportance.Low, $"Compressed {path} into the zip file as {zipName}");
		}

		static int GetExternalAttributes (ZipArchiveEntry self)
		{
			// The ZipArchiveEntry.ExternalAttributes property is available in .NET 4.7.2 (which we need to target for builds on Windows) and .NET 5+, but not netstandard2.0 (which is the latest netstandard .NET 4.7.2 supports).
			// Since the property will always be available at runtime, just call it using reflection.
#if NET
			return self.ExternalAttributes;
#else
			var property = typeof (ZipArchiveEntry).GetProperty ("ExternalAttributes", BindingFlags.Instance | BindingFlags.Public);
			return (int) property.GetValue (self);
#endif
		}

	}
}

