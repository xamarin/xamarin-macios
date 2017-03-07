using Xamarin.MacDev.Tasks;

namespace Microsoft.Build.Tasks
{
	public class RemoveDir : RemoveDirBase
	{
		public override bool Execute ()
		{
			Log.LogTaskName ("RemoveDir");
			Log.LogTaskProperty ("Directories", Directories);

			return base.Execute ();
		}
	}
}
