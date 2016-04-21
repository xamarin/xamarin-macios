using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks {
	public class ACTool : ACToolTaskBase
	{
		protected override string MinimumDeploymentTargetKey {
			get { return ManifestKeys.LSMinimumSystemVersion; }
		}
	}
}
