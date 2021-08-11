using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks
{
	public class GetMinimumOSVersion : GetMinimumOSVersionTaskBase
	{
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}
	}
}
