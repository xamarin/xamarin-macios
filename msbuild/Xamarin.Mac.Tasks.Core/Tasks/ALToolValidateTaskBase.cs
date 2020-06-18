using System.Text;

namespace Xamarin.Mac.Tasks
{
	public abstract class ALToolValidateTaskBase : MacDev.Tasks.ALToolTaskBase
	{
		protected override string GenerateCommandLineCommands ()
		{
			var sb = new StringBuilder ();
			sb.Append ("--validate-app ");
			sb.Append (base.GenerateCommandLineCommands ());

			return sb.ToString ();
		}
	}
}
