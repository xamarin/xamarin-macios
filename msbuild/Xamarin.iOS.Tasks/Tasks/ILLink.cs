using ILLink.Tasks;
using Microsoft.Build.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks 
{
	public class ILLink : ILLinkBase
	{
		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public override void Cancel ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
			else
				base.Cancel ();
		}
	}
}
