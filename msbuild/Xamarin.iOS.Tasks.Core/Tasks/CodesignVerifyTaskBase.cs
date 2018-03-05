using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class CodesignVerifyTaskBase : Xamarin.MacDev.Tasks.CodesignVerifyTaskBase
	{
		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("--verify");
			args.Add ("-vvvv");
			args.Add ("-R='anchor apple generic and certificate 1[field.1.2.840.113635.100.6.2.1] exists and (certificate leaf[field.1.2.840.113635.100.6.1.2] exists or certificate leaf[field.1.2.840.113635.100.6.1.4] exists)'");

			args.AddQuoted (Resource);

			return args.ToString ();
		}
	}
}
