
#if !WATCH
using XamCore.UIKit;
using XamCore.Foundation;

namespace XamCore.UIKit {
	public partial class UIPopoverPresentationControllerDelegate 
	{
		// this is a workaround to allow exposing the old API ()
		[Export ("adaptivePresentationStyleForPresentationController:")]
		public virtual UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController forPresentationController)
		{
			throw new You_Should_Not_Call_base_In_This_Method ();
		}
	}
}

#endif
