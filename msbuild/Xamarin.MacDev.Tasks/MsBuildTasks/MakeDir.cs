using Xamarin.MacDev.Tasks;

namespace Microsoft.Build.Tasks
{
	public class MakeDir : MakeDirBase
	{
		public override bool Execute ()
		{
			Log.LogTaskName ("MakeDir");
			Log.LogTaskProperty ("Directories", Directories);

			return base.Execute ();
		}
	}
}
