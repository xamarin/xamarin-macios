using Microsoft.Build.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks 
{
	public class ILStrip : ILStripTasks.ILStripBase
	{
		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}
	}
}
