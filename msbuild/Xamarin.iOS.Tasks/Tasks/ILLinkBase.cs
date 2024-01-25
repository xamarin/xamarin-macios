using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.iOS.Tasks {
	public class ILLink : global::ILLink.Tasks.ILLink {
		public string SessionId { get; set; } = string.Empty;

		public ITaskItem [] DebugSymbols { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string LinkerItemsDirectory { get; set; } = string.Empty;

		[Output]
		public ITaskItem [] LinkerOutputItems { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] LinkedItems { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var result = base.Execute ();

			var linkerItems = new List<ITaskItem> ();
			var linkedItems = new List<ITaskItem> ();

			if (result) {
				// Adds all the files in the linker-items dir
				foreach (var item in Directory.EnumerateFiles (LinkerItemsDirectory)) {
					linkerItems.Add (new TaskItem (item));
				}

				// Adds all the files in the linked output dir
				foreach (var item in Directory.EnumerateFiles (OutputDirectory.ItemSpec)) {
					linkedItems.Add (new TaskItem (item));
				}
			}

			LinkerOutputItems = linkerItems.ToArray ();
			LinkedItems = linkedItems.ToArray ();

			return result;
		}

		public override void Cancel ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			else
				base.Cancel ();
		}
	}
}
