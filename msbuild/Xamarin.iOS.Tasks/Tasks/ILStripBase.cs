using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class ILStrip : global::ILStrip, ITaskCallback {
		public string SessionId { get; set; } = string.Empty;

		[Output]
		public ITaskItem [] StrippedAssemblies { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var result = base.Execute ();

			var stripedItems = new List<ITaskItem> ();

			if (result) {
				foreach (var item in Assemblies) {
					stripedItems.Add (new TaskItem (item.GetMetadata ("OutputPath"), item.CloneCustomMetadata ()));
				}
			}

			StrippedAssemblies = stripedItems.ToArray ();

			return result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
