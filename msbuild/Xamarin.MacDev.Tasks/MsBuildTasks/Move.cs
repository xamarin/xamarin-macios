using Xamarin.MacDev.Tasks;

namespace Microsoft.Build.Tasks
{
	public class Move : MoveTaskBase
	{
		public override bool Execute ()
		{
			Log.LogTaskName ("Move");
			Log.LogTaskProperty ("DestinationFiles", DestinationFiles);
			Log.LogTaskProperty ("DestinationFolder", DestinationFolder);
			Log.LogTaskProperty ("SourceFiles", SourceFiles);

			return base.Execute ();
		}
	}
}
