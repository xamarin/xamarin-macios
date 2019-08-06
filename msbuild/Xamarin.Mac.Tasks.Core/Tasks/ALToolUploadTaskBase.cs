using System.Text;

using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks
{
	public class ALToolUploadTaskBase : Xamarin.MacDev.Tasks.ALToolTaskBase
	{
		public override PlatformName FileType => PlatformName.MacOSX;

		protected override string GenerateCommandLineCommands ()
		{
			var sb = new StringBuilder ();
			sb.Append ("--upload-app ");
			sb.Append (base.GenerateCommandLineCommands ());

			return sb.ToString ();
		}
	}
}
