using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.iOS.Tasks {
	public class CreateArchiveDirectory : Task
	{
		#region Inputs

		[Required]
		public string ArchiveBasePath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string ArchiveRootDir { get; set; }

		#endregion

		public override bool Execute ()
		{
			var expandedBasePath = Environment.ExpandEnvironmentVariables (ArchiveBasePath);
			ArchiveRootDir = Path.Combine(expandedBasePath, DateTime.Now.ToString ("yyyy-MM-dd"));

			if (!Directory.Exists (ArchiveRootDir)) {
				Directory.CreateDirectory (ArchiveRootDir);
			}

			return true;
		}
	}
}
