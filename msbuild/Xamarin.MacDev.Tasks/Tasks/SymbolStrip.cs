using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks
{
	public class SymbolStrip : SymbolStripTaskBase
	{
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}
	}
}
