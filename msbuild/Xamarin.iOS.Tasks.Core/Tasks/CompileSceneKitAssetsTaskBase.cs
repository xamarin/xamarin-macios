namespace Xamarin.iOS.Tasks
{
	public abstract class CompileSceneKitAssetsTaskBase : Xamarin.MacDev.Tasks.CompileSceneKitAssetsTaskBase
	{
		protected override string OperatingSystem {
			get { return "ios"; }
		}
	}
}
