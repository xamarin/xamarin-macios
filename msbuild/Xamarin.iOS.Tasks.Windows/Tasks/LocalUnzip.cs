using Ionic.Zip;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using Xamarin.iOS.Tasks.Windows.Properties;

namespace Xamarin.iOS.Tasks.Windows {
	public class LocalUnzip : Task {
		[Required]
		public string ZipFilePath { get; set; }
		[Required]
		public string ExtractionPath { get; set; }

		public override bool Execute ()
		{
			LogTaskProperty ("ZipFilePath", ZipFilePath);
			LogTaskProperty ("ExtractionPath", ExtractionPath);

			try {
				Log.LogMessage (Resources.LocalUnzip_Unzipping, ZipFilePath);

				// Extra check to ensure the unzip won't fail if a file exists with the same path
				if (File.Exists (ExtractionPath))
					File.Delete (ExtractionPath);

				using (var zipFile = ZipFile.Read (ZipFilePath))
					zipFile.ExtractAll (ExtractionPath, ExtractExistingFileAction.OverwriteSilently);

				// Fixes last write time of all files in the Zip, because the files keep the last write time from the Mac.
				foreach (var filePath in Directory.EnumerateFiles (ExtractionPath, "*.*", SearchOption.AllDirectories))
					new FileInfo (filePath).LastWriteTime = DateTime.Now;

				Log.LogMessage (Resources.LocalUnzip_Unzipped, ZipFilePath);
			} catch (Exception ex) {
				Log.LogError (Resources.LocalUnzip_Error, ZipFilePath, ex.Message);
			}

			return !Log.HasLoggedErrors;
		}

		//TODO: Ideally we should get this from the LoggingExtensions in Xamarin.MacDev.Tasks. We would need the reference for that
		void LogTaskProperty (string propertyName, string value)
		{
			Log.LogMessage (MessageImportance.Normal, "  {0}: {1}", propertyName, value ?? "<null>");
		}
	}
}
