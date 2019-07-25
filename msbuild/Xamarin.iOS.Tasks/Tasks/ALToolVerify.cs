using System.Text;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public class ALToolVerify : ALToolTaskBase
	{
        public override PlatformName FileType => PlatformName.iOS;

		protected override string GenerateCommandLineCommands ()
		{
			var sb = new StringBuilder ();
			sb.Append ("--validate-app ");
			sb.Append (base.GenerateCommandLineCommands ());

			return sb.ToString ();
		}
	}
}
