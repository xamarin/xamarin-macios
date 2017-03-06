using Xamarin.MacDev.Tasks;

namespace Microsoft.Build.Tasks
{
	public class Touch : TouchBase
	{
		public override bool Execute ()
		{
			Log.LogTaskName ("Touch");
			Log.LogTaskProperty ("AlwaysCreate", AlwaysCreate);
			Log.LogTaskProperty ("Files", Files);
			Log.LogTaskProperty ("ForceTouch", ForceTouch);
			Log.LogTaskProperty ("Time", Time);

			return base.Execute ();
		}
	}
}