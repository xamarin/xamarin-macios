using Xamarin.MacDev.Tasks;

namespace Microsoft.Build.Tasks
{
	public class Copy : CopyBase
	{
		public override bool Execute ()
		{
			Log.LogTaskName ("Copy");
			Log.LogTaskProperty ("DestinationFiles", DestinationFiles);
			Log.LogTaskProperty ("DestinationFolder", DestinationFolder);
			Log.LogTaskProperty ("OverwriteReadOnlyFiles", OverwriteReadOnlyFiles);
			Log.LogTaskProperty ("Retries", Retries);
			Log.LogTaskProperty ("RetryDelayMilliseconds", RetryDelayMilliseconds);
			Log.LogTaskProperty ("SkipUnchangedFiles", SkipUnchangedFiles);
			Log.LogTaskProperty ("SourceFiles", SourceFiles);
			Log.LogTaskProperty ("UseHardlinksIfPossible", UseHardlinksIfPossible);

			return base.Execute ();
		}
	}
}
