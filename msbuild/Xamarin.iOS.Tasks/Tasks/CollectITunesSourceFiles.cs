using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.iOS.Tasks {
	public class CollectITunesSourceFiles : XamarinTask, ICancelableTask {
		static readonly string [] iTunesFileNames = { "iTunesMetadata.plist", "iTunesArtwork@2x", "iTunesArtwork" };

		#region Inputs

		[Required]
		public string OutputPath { get; set; } = string.Empty;

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] ITunesSourceFiles { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

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

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
