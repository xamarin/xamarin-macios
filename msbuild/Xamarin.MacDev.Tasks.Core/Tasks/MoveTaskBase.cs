using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class MoveTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		public ITaskItem[] DestinationFiles { get; set; }

		public ITaskItem DestinationFolder { get; set; }

		[Required]
		public ITaskItem[] SourceFiles { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] MovedFiles { get; set; }

		#endregion

		public override bool Execute ()
		{
			if (DestinationFolder != null && DestinationFiles != null) {
				Log.LogError ("Cannot specify both DestinationFiles and DestinationFolder.");
				return false;
			}

			if (DestinationFolder == null && DestinationFiles == null) {
				Log.LogError ("DestinationFolder and DestinationFiles cannot both be empty.");
				return false;
			}

			var moved = new List<ITaskItem> ();

			if (DestinationFiles != null) {
				if (DestinationFiles.Length != SourceFiles.Length) {
					Log.LogError ("The number of SourceFiles and DestinationFiles must match.");
					return false;
				}

				for (int i = 0; i < SourceFiles.Length; i++) {
					try {
						Directory.CreateDirectory (Path.GetDirectoryName (DestinationFiles[i].ItemSpec));

						var destination = DestinationFiles[i].ItemSpec;
						if (File.Exists(destination))
							File.Delete(destination);

						File.Move (SourceFiles[i].ItemSpec, destination);
						moved.Add (DestinationFiles[i]);
					} catch (Exception ex) {
						Log.LogError ("Failed to move '{0}' to '{1}': {2}", SourceFiles[i].ItemSpec, DestinationFiles[i].ItemSpec, ex.Message);
					}
				}
			} else {
				try {
					Directory.CreateDirectory (DestinationFolder.ItemSpec);
				} catch (Exception ex) {
					Log.LogError ("Failed to create DestinationFolder '{0}': {1}", DestinationFolder.ItemSpec, ex.Message);
					return false;
				}

				for (int i = 0; i < SourceFiles.Length; i++) {
					var destination = Path.Combine (DestinationFolder.ItemSpec, Path.GetFileName (SourceFiles[i].ItemSpec));

					try {
						if (File.Exists(destination))
							File.Delete(destination);
						
						File.Move (SourceFiles[i].ItemSpec, destination);
						moved.Add (new TaskItem (destination));
					} catch (Exception ex) {
						Log.LogError ("Failed to move '{0}' to '{1}': {2}", SourceFiles[i].ItemSpec, destination, ex.Message);
					}
				}
			}

			MovedFiles = moved.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
