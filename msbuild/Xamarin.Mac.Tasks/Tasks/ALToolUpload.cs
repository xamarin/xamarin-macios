using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class ALToolUpload : ALToolTaskBase
	{
		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();
			args.Add ("--upload-app");

			return args.ToString ();
		}
	}
}
