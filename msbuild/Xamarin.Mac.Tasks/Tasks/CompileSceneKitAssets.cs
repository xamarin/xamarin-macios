using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class CompileSceneKitAssets : CompileSceneKitAssetsTaskBase
	{
		protected override string OperatingSystem {
			get { return "osx"; }
		}
	}
}
