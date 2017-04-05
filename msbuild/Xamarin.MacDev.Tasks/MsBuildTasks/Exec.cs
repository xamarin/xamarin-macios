using Xamarin.MacDev.Tasks;

namespace Microsoft.Build.Tasks
{
	public class Exec : ExecBase
	{
		public override bool Execute ()
		{
			Log.LogTaskName ("Exec");
			Log.LogTaskProperty ("Command", Command);
			Log.LogTaskProperty ("CustomErrorRegularExpression", CustomErrorRegularExpression);
			Log.LogTaskProperty ("CustomWarningRegularExpression", CustomWarningRegularExpression);
			Log.LogTaskProperty ("IgnoreExitCode", IgnoreExitCode);
			Log.LogTaskProperty ("IgnoreStandardErrorWarningFormat", IgnoreStandardErrorWarningFormat);
			Log.LogTaskProperty ("StdErrEncoding", StdErrEncoding);
			Log.LogTaskProperty ("StdOutEncoding", StdOutEncoding);
			Log.LogTaskProperty ("WorkingDirectory", WorkingDirectory);

			return base.Execute ();
		}
	}
}