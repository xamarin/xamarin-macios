using System;
using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks
{
	public class Copy : CopyBase
	{
		public override bool Execute ()
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT || string.IsNullOrEmpty (SessionId))
				return base.Execute ();

			var taskRunner = new TaskRunner (SessionId, BuildEngine4);

			taskRunner.FixReferencedItems (SourceFiles);

			return taskRunner.RunAsync (this).Result;
		}
	}
}
