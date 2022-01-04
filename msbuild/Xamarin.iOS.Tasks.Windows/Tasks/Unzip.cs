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
				Log.LogMessage (Resources.LocalUnzip_Unzipping, ZipFilePath);

				//Directory.Delete will fail if a file with the same path already exist
				if (File.Exists (ExtractionPath)) {
					File.Delete (ExtractionPath);
				}

				//ZipFile.ExtractToDirectory will fail if the directory already exist
				if (Directory.Exists (ExtractionPath)) {
					Directory.Delete (ExtractionPath, recursive: true);
				}

				ZipFile.ExtractToDirectory (ZipFilePath, ExtractionPath);

				// Fixes last write time of all files in the Zip, because the files keep the last write time from the Mac.
				foreach (var filePath in Directory.EnumerateFiles (ExtractionPath, "*.*", SearchOption.AllDirectories)) {
					new FileInfo (filePath).LastWriteTime = DateTime.Now;
				}

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
