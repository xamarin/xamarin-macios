using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks
{
	public class IBTool : IBToolTaskBase, ICancelableTask
	{
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public void Cancel ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}
	}
}
