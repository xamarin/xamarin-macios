using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public class CollectITunesSourceFilesTaskBase : XamarinTask
	{
		static readonly string[] iTunesFileNames = { "iTunesMetadata.plist", "iTunesArtwork@2x", "iTunesArtwork" };

		#region Inputs

		[Required]
		public string OutputPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] ITunesSourceFiles { get; set; }

		#endregion

		public override bool Execute ()
		{
			var files = new List<ITaskItem> ();

			foreach (var fileName in iTunesFileNames) {
				var file = Path.Combine (OutputPath, fileName);

				if (File.Exists (file))
					files.Add (new TaskItem (file));
			}

			ITunesSourceFiles = files.ToArray ();

			Log.LogTaskProperty ("ITunesSourceFiles", ITunesSourceFiles);

			return !Log.HasLoggedErrors;
		}
	}
}
