using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class CodesignVerify : CodesignVerifyTaskBase
	{
		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("--verify");
			args.Add ("-vvvv");
			args.Add ("--deep");

			args.AddQuoted (Resource);

			return args.ToString ();
		}
	}
}
