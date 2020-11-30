using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks
{
	public class LinkNativeCode : LinkNativeCodeTaskBase
	{
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}
	}
}

