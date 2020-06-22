using System.Text;

namespace Xamarin.iOS.Tasks
{
	public abstract class ALToolUploadTaskBase : MacDev.Tasks.ALToolTaskBase
	{
		protected override string GenerateCommandLineCommands ()
		{
			var sb = new StringBuilder ();
			sb.Append ("--upload-app ");
			sb.Append (base.GenerateCommandLineCommands ());

			return sb.ToString ();
		}
	}
}
