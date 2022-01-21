using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using Xamarin.iOS.Tasks.Windows.Properties;

namespace Xamarin.iOS.Tasks.Windows {
	public class Unzip : Task {
		[Required]
		public string ZipFilePath { get; set; }

		[Required]
		public string ExtractionPath { get; set; }

		public override bool Execute ()
		{
			LogTaskProperty ("ZipFilePath", ZipFilePath);
			LogTaskProperty ("ExtractionPath", ExtractionPath);

			try {
				Log.LogMessage (Resources.Unzip_Unzipping, ZipFilePath);

				// We use a temp dir because the extraction dir should not exist for the ZipFile API to work
				var tempExtractionPath = Path.Combine (Path.GetTempPath(), "Xamarin", "Archive", Guid.NewGuid().ToString());

				ZipFile.ExtractToDirectory (ZipFilePath, tempExtractionPath);

				CopyDirectory (tempExtractionPath, ExtractionPath);

				// Fixes last write time of all files in the Zip, because the files keep the last write time from the Mac.
				foreach (var filePath in Directory.EnumerateFiles (ExtractionPath, "*.*", SearchOption.AllDirectories)) {
					new FileInfo (filePath).LastWriteTime = DateTime.Now;
				}

				Directory.Delete (tempExtractionPath, true);

				Log.LogMessage (Resources.Unzip_Unzipped, ZipFilePath);
			} catch (Exception ex) {
				Log.LogError (Resources.Unzip_Error, ZipFilePath, ex.Message);
			}

			return !Log.HasLoggedErrors;
		}

		//TODO: Ideally we should get this from the LoggingExtensions in Xamarin.MacDev.Tasks. We would need the reference for that
		void LogTaskProperty (string propertyName, string value)
		{
			Log.LogMessage (MessageImportance.Normal, "  {0}: {1}", propertyName, value ?? "<null>");
		}

		void CopyDirectory (string sourceDir, string destDir)
		{
			var sourceDirInfo = new DirectoryInfo (sourceDir);

			Directory.CreateDirectory (destDir);

			foreach (var file in sourceDirInfo.GetFiles ()) {
				file.CopyTo (Path.Combine (destDir, file.Name));
			}

			foreach (var dir in sourceDirInfo.GetDirectories()) {
				CopyDirectory (dir.FullName, Path.Combine (destDir, dir.Name));
			}
		}
	}
}
