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

				Zip.Extract (ZipFilePath, ExtractionPath);

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
	}
}
