using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks {
	public class IBTool : IBToolTaskBase
	{
		protected override bool AutoActivateCustomFonts {
			get { return false; }
		}
	}
}
