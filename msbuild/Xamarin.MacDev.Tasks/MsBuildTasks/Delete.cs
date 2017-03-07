using Xamarin.MacDev.Tasks;

namespace Microsoft.Build.Tasks
{
	public class Delete : DeleteBase
	{
		public override bool Execute ()
		{
			Log.LogTaskName ("Delete");
			Log.LogTaskProperty ("Files", Files);
			Log.LogTaskProperty ("TreatErrorsAsWarnings", TreatErrorsAsWarnings);

			return base.Execute ();
		}
	}
}
