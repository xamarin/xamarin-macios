#if !XAMCORE_3_0

using System;

namespace UIKit {

	public partial class UIActivityViewController {
		[Obsolete ("iOS 9 does not allow creating an empty instance.")]
		public UIActivityViewController ()
		{
		}

		[Obsolete ("Use 'CompletionWithItemsHandler' property.")]
		public virtual void SetCompletionHandler (UIActivityViewControllerCompletion completionHandler)
		{
			CompletionWithItemsHandler = completionHandler;
		}
	}
}

#endif
