using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class ALToolValidate : ALToolTaskBase
	{
		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();
			args.Add ("--validate-app");

			return args.ToString ();
		}
	}
}
