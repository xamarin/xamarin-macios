using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;

namespace Xamarin.iOS.Tasks.Windows {
	public class CreateArchiveDirectory : Task {
		[Required]
		public string ArchiveBasePath { get; set; } = string.Empty;

		[Output]
		public string ArchiveRootDir { get; set; } = string.Empty;

		public override bool Execute ()
		{
			var expandedBasePath = Environment.ExpandEnvironmentVariables (ArchiveBasePath);

			ArchiveRootDir = Path.Combine (expandedBasePath, DateTime.Now.ToString ("yyyy-MM-dd"));

			try {
				Directory.CreateDirectory (ArchiveRootDir);
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}

			return true;
		}
	}
}
