using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class ScnTool : ScnToolTaskBase
	{
		protected override string OperatingSystem {
			get { return "osx"; }
		}
	}
}
