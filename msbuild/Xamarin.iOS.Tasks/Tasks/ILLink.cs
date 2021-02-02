using Xamarin.Messaging.Build.Client;
using ILLink.Tasks;

namespace Xamarin.iOS.Tasks
{
	public class ILLink : ILLinkBase
	{
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public override void Cancel ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
			else
				base.Cancel ();
		}
	}
}
