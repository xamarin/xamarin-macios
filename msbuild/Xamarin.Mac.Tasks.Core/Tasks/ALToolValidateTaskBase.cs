using System.Text;

using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks
{
	public class ALToolValidateTaskBase : Xamarin.MacDev.Tasks.ALToolTaskBase
	{
        public override PlatformName FileType => PlatformName.MacOSX;
		
		protected override string GenerateCommandLineCommands ()
		{
			var sb = new StringBuilder ();
			sb.Append ("--validate-app ");
			sb.Append (base.GenerateCommandLineCommands ());

			return sb.ToString ();
		}
	}
}
